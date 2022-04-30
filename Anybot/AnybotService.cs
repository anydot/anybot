using Anybot.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Anybot
{
    public class AnybotService : IHostedService, IUpdateHandler
    {
        private readonly ITelegramBotClient bot;
        private readonly ICommand[] commands;
        private CancellationTokenSource? cts;
        private readonly ILogger<AnybotService> logger;
        private string botPostfix = "";

        public AnybotService(ITelegramBotClient bot, ICommand[] commands, ILogger<AnybotService> logger)
        {
            this.bot = bot;
            this.commands = commands;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var botDetails = await bot.GetMeAsync(cts.Token).ConfigureAwait(false);
            botPostfix = $"@{botDetails.Username}";

            logger.LogInformation($"Starting bot: {botDetails.Username}");
            logger.LogInformation("Starting receiving commands: {0}", string.Join(',', commands.Select(c => c.CommandName)));

            await bot.SetMyCommandsAsync(commands.Select(c => new BotCommand { Command = c.CommandName, Description = c.CommandDescription }), cancellationToken: cts.Token).ConfigureAwait(false);

            bot.StartReceiving(this, cancellationToken: cts.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cts?.Cancel();
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.ChannelPost || update.Type == UpdateType.Message)
            {
                var message = update.Message ?? update.ChannelPost;

                if (message?.Entities != null && message?.Text != null)
                {
                    var botcommandEntity = Array.Find(message.Entities, e => e.Type == MessageEntityType.BotCommand);

                    if (botcommandEntity != null)
                    {
                        var command = message.Text.Substring(botcommandEntity.Offset + 1, botcommandEntity.Length - 1);

                        if (command.EndsWith(botPostfix))
                        {
                            command = command[..^botPostfix.Length];
                        }

                        var commandMatch = Array.Find(commands, c => c.CommandName == command);

                        if (commandMatch != null)
                        {
                            await commandMatch.HandleUpdate(botClient, update).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Got exception");
            return Task.CompletedTask;
        }
    }
}