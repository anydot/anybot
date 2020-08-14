using Anybot.Common;
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
        where T : Tag, new()
    {
        private readonly IRocksWrapper<RoumenImage<T>> db;
        private readonly ITelegramBotClient bot;
        private readonly ILogger<RoumenService<T>> logger;
        private readonly IRoumenRestService<T> roumenRestService;
        private readonly TimeSpan refreshDelay;
        private readonly IDelayer delayer;
        private readonly IOptions<RoumenOptions<T>> options;

        public RoumenService(IOptions<RoumenOptions<T>> options, ILogger<RoumenService<T>> logger, IRocksWrapper<RoumenImage<T>> db, ITelegramBotClient bot, IRoumenRestService<T> roumenRestService, IDelayer delayer)
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

                if (!db.TryRead(image.ImageUrl, out var _))
                {
                    logger.LogDebug($"New image {image.ImageUrl}");

                    if (!options.Value.Silent)
                    {
                        try
                        {
                            await delayer.Delay(async () => await bot.MessageWithOptionalImage((long)options.Value.ChatId!, FormatImage(image), image.ImageUrl).ConfigureAwait(false)).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, $"Can't process image {image}");
                        }
                    }
                    db.Write(image.ImageUrl, image);
                }
                else
                {
                    logger.LogDebug($"Skipping existing image {image.ImageUrl}");
                }
            }

            logger.LogInformation("Cleanup");

            foreach (var dbImage in db.Iterate())
            {
                if (!allImages.Contains(dbImage.Value))
                {
                    logger.LogDebug($"Removing stale {dbImage.Key}");
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