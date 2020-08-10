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
    public class RoumenService : BackgroundService
    {
        private readonly IRocksWrapper<RoumenImage> db;
        private readonly ITelegramBotClient bot;
        private readonly ILogger<RoumenService> logger;
        private readonly IRoumenRestService roumenRestService;
        private readonly TimeSpan refreshDelay;
        private readonly IDelayer delayer;
        private readonly IOptions<RoumenOptions> options;

        public RoumenService(IOptions<RoumenOptions> options, ILogger<RoumenService> logger, IRocksWrapper<RoumenImage> db, ITelegramBotClient bot, IRoumenRestService roumenRestService, IDelayer delayer)
        {
            _ = options.Value.ChatId ?? throw new ArgumentNullException("Bot.chatId");
            refreshDelay = options.Value.RefreshDelay;

            this.logger = logger;
            this.db = db;
            this.bot = bot;
            this.roumenRestService = roumenRestService;
            this.delayer = delayer;
            this.options = options;
        }

        public string FormatImage(RoumenImage p)
        {
            return string.Format("[{0}]({1})", bot.Quote(p.Description), bot.Quote(p.CommentLink));
        }

        private async Task RunOnce(CancellationToken cancellationToken)
        {
            var allImages = (await roumenRestService.FetchImagesFromWeb().ConfigureAwait(false)).Reverse().ToList();

            foreach (var image in allImages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                logger.LogDebug($"New image {image.ImageUrl}");

                if (!db.TryRead(image.ImageUrl, out var _))
                {
                    if (!options.Value.Silent)
                    {
                        await delayer.Delay(async () => await bot.MessageWithOptionalImage((long)options.Value.ChatId!, FormatImage(image), image.ImageUrl).ConfigureAwait(false)).ConfigureAwait(false);
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