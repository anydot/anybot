using Anybot.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ActivePass
{
    public class ActivePassService : BackgroundService
    {
        private enum PartnerDbMatch
        {
            unused,
            NotFound,
            FoundExactMatch,
            FoundDifference
        }

        private readonly IRocksWrapper<Partner> db;
        private readonly ITelegramBotClient bot;
        private readonly ILogger<ActivePassService> logger;
        private readonly IActivePassRestService activePassRestService;
        private readonly TimeSpan refreshDelay;
        private readonly IDelayer delayer;
        private readonly IOptions<BotOptions> options;

        public ActivePassService(IOptions<BotOptions> options, ILogger<ActivePassService> logger, IRocksWrapper<Partner> db, ITelegramBotClient bot, IActivePassRestService activePassRestService, IDelayer delayer)
        {
            _ = options.Value.ChatId ?? throw new ArgumentOutOfRangeException(nameof(options), "options don't contain the chatID");
            refreshDelay = options.Value.RefreshDelay;

            this.logger = logger;
            this.db = db;
            this.bot = bot;
            this.activePassRestService = activePassRestService;
            this.delayer = delayer;
            this.options = options;
        }

        private string FormatPartner(Partner p, string prefix)
        {
            StringBuilder sb = new StringBuilder(prefix);

            sb.AppendFormat("[{0}]({1})\n", bot.Quote(p.Company), bot.Quote("https://www.activepass.cz/partner/" + p.PartnerId));
            sb.AppendFormat("{0}\n{1}", bot.Quote(p.Address), bot.Quote(p.District));
            if (p.Website != null)
            {
                sb.AppendJoin(null, "\n", bot.Quote(p.Website));
            }

            return sb.ToString();
        }

        static public bool TryFormatImage(Partner p, out string? result)
        {
            if (p.ImageUrl == null)
            {
                result = default;
                return false;
            }

            result = $"https://www.activepass.cz/files/images/{p.ImageUrl}";
            return true;
        }

        internal async Task RunOnce()
        {
            var allPartners = await activePassRestService.FetchPartnersFromWeb().ConfigureAwait(false);

            var allPartnersDb = db.Iterate().Select(kv => kv.Value).ToHashSet();
            var allPartnersIds = allPartnersDb.Select(p => p.PartnerId).ToHashSet();
            var partnersExistLookup = allPartners.ToLookup(p =>
            {
                if (!allPartnersIds.Contains(p.PartnerId))
                {
                    return PartnerDbMatch.NotFound;
                }

                if (allPartnersDb.Contains(p))
                {
                    return PartnerDbMatch.FoundExactMatch;
                }

                return PartnerDbMatch.FoundDifference;
            });

            if (partnersExistLookup.Contains(PartnerDbMatch.NotFound))
            {
                await ProcessPartners(partnersExistLookup[PartnerDbMatch.NotFound], string.Empty, p => db.Write(p.PartnerId, p)).ConfigureAwait(false);
            }

            if (partnersExistLookup.Contains(PartnerDbMatch.FoundDifference))
            {
                await ProcessPartners(partnersExistLookup[PartnerDbMatch.FoundDifference], "*Updated*\n", p => db.Write(p.PartnerId, p)).ConfigureAwait(false);
            }

            await ProcessPartners(allPartnersDb.Where(p => !allPartners.Any(pp => pp.PartnerId == p.PartnerId)), "*Removed*\n", p => db.Delete(p.PartnerId)).ConfigureAwait(false);

            logger.LogInformation("Done");
        }

        private async Task ProcessPartners(IEnumerable<Partner> partners, string prefix, Action<Partner> action)
        {
            foreach (var partner in partners)
            {
                logger.LogInformation($"Processing partner {partner.PartnerId}, prefix {prefix}");
                logger.LogDebug(partner.ToString());

                try
                {
                    TryFormatImage(partner, out var imageUrl);

                    if (!options.Value.Silent)
                    {
                        await delayer.Delay(async () => await bot.MessageWithOptionalImage((long)options.Value.ChatId!, FormatPartner(partner, prefix), imageUrl).ConfigureAwait(false)).ConfigureAwait(false);
                    }

                    action(partner);
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException e)
                {
                    logger.LogError(e, "Caught exception while processing partner");
                }
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RunOnce().ConfigureAwait(false);
                await Task.Delay(refreshDelay, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}