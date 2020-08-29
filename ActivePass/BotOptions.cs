using System;

namespace ActivePass
{
    public class BotOptions
    {
        public long? ChatId { get; set; }
        public string? DataUrl { get; set; }
        public TimeSpan RefreshDelay { get; set; } = TimeSpan.FromHours(23);
        public bool Silent { get; set; }
    }
}
