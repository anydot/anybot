﻿using Anybot.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RoumenBot
{
    public class RoumenService<T> : BackgroundService
        where T : ITag, new()
    {
        private readonly IStorage<RoumenImage<T>> db;
        private readonly ITelegramBotClient bot;
        private readonly ILogger<RoumenService<T>> logger;
        private readonly IRoumenRestService<T> roumenRestService;
        private readonly TimeSpan refreshDelay;
        private readonly IDelayer delayer;
        private readonly IOptions<RoumenOptions<T>> options;

        public RoumenService(IOptions<RoumenOptions<T>> options, ILogger<RoumenService<T>> logger, IStorage<RoumenImage<T>> db, ITelegramBotClient bot, IRoumenRestService<T> roumenRestService, IDelayer delayer)
        {
            _ = options.Value.ChatId ?? throw new ArgumentOutOfRangeException(nameof(options), "options don't contain the chatID");
            refreshDelay = options.Value.RefreshDelay;

            this.logger = logger;
            this.db = db;
            this.bot = bot;
            this.roumenRestService = roumenRestService;
            this.delayer = delayer;
            this.options = options;
        }

        public string FormatImage(RoumenImage<T> p)
        {
            return string.Format("[{0}]({1})", bot.Quote(p.Description), bot.Quote(p.CommentLink));
        }

        private async Task RunOnce(CancellationToken cancellationToken)
        {
            var allImages = (await roumenRestService.FetchImagesFromWeb().ConfigureAwait(false)).Reverse().ToList();

            foreach (var image in allImages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                bool writeToDb = true;

                if (!db.TryRead(image.ImageUrl, out var _))
                {
                    logger.LogDebug("New image {0}", image.ImageUrl);

                    if (!options.Value.Silent)
                    {
                        try
                        {
                            await delayer.Delay(async () => await bot.MessageWithOptionalImage((long)options.Value.ChatId!, FormatImage(image), image.ImageUrl).ConfigureAwait(false)).ConfigureAwait(false);
                        }
                        catch (Telegram.Bot.Exceptions.ApiRequestException e) when (e.Message.Contains("wrong file identifier/HTTP URL specified"))
                        {
                            logger.LogInformation(e, "Ignoring image publish {0}", image.ImageUrl);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, "Can't process image {0}", image);
                            writeToDb = false;
                        }
                    }

                    if (writeToDb)
                    {
                        db.Write(image.ImageUrl, image);
                    }
                }
            }

            logger.LogInformation("Cleanup");

            foreach (var dbImage in db.Iterate())
            {
                var value = dbImage.Value();
                if (value == null || !allImages.Contains(value))
                {
                    logger.LogDebug("Removing stale {0}", dbImage.Key);
                    db.Delete(dbImage.Key);
                }
            }

            logger.LogInformation("Run done");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnce(stoppingToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Run resulted in exception");
                }

                await Task.Delay(refreshDelay, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}