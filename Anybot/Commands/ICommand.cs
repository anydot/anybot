using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Anybot.Commands
{
    public interface ICommand
    {
        public abstract string CommandName { get; }
        public abstract string CommandDescription { get; }

        public abstract Task HandleUpdate(ITelegramBotClient bot, Update update);
    }
}
