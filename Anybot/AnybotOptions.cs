using System;

namespace Anybot
{
    public class AnybotOptions
    {
        public string? Token { get; set; }
        public TimeSpan PostDelay { get; set; } = TimeSpan.FromSeconds(5);
        public string Database { get; set; } = "anybot.db";
        public TimeSpan DatabaseCompact { get; set; } = TimeSpan.FromHours(12);
    }
}
