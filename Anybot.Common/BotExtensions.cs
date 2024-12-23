using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Anybot.Common
{
    public static class BotExtensions
    {
        private static readonly LinkPreviewOptions NoPreview = new() { IsDisabled = true };

        public static async Task MessageWithOptionalImage(this ITelegramBotClient bot, long chatid, string message, string? imageUrl)
        {
            if (imageUrl == null)
            {
                await bot.SendMessage(chatid, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, linkPreviewOptions: NoPreview).ConfigureAwait(false);
            }
            else
            {
                await bot.SendPhoto(chatid, InputFile.FromUri(imageUrl), caption: message, parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2).ConfigureAwait(false);
            }
        }

        static public string Quote(this ITelegramBotClient _, string text)
        {
            return Regex.Replace(text, "([_*\\[\\]()~`>#+=|{}.!-])", "\\$1");
        }
    }
}