﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Anybot.Common
{
    public static class BotExtensions
    {
        public static async Task MessageWithOptionalImage(this ITelegramBotClient bot, long chatid, string message, string? imageUrl)
        {
            if (imageUrl == null)
            {
                await bot.SendTextMessageAsync(chatid, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, disableWebPagePreview: true).ConfigureAwait(false);
            }
            else
            {
                await bot.SendPhotoAsync(chatid, InputFile.FromUri(imageUrl), caption: message, parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2).ConfigureAwait(false);
            }
        }

        static public string Quote(this ITelegramBotClient _, string text)
        {
            return Regex.Replace(text, "([_*\\[\\]()~`>#+=|{}.!-])", "\\$1");
        }
    }
}