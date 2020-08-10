using System;

namespace RoumenBot
{
    public class RoumenOptions
    {
        public long? ChatId { get; set; }
        public string? DataUrl { get; set; }
        public TimeSpan RefreshDelay { get; set; } = TimeSpan.FromMinutes(47);
        public bool Silent { get; set; } = false;
    }
}
