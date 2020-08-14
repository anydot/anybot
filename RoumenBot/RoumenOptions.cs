using System;

namespace RoumenBot
{
#pragma warning disable S2326 // Unused type parameters should be removed
    public class RoumenOptions<T> where T : Tag
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        public long? ChatId { get; set; }
        public string? DataUrl { get; set; }
        public TimeSpan RefreshDelay { get; set; } = TimeSpan.FromMinutes(47);
        public bool Silent { get; set; } = false;
    }
}
