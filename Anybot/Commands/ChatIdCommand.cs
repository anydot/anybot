using Anybot.Common;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Anybot.Commands
{
    public class ChatIdCommand : ICommand
    {
        private readonly IDelayer delayer;

        public ChatIdCommand(IDelayer delayer)
        {
            this.delayer = delayer;
        }

        public string CommandName { get; } = "chatid";
        public string CommandDescription { get; } = "Get chatid of current chat";

        public async Task HandleUpdate(ITelegramBotClient bot, Update update)
        {
            Message message = update.Message ?? update.ChannelPost;

            if (message == null)
            {
                return;
            }

            await delayer.Delay(async () => await bot.SendTextMessageAsync(message.Chat.Id, $"Id of this chat: {message.Chat.Id}", replyToMessageId: message.MessageId).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
