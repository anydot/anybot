using Anybot.Common;
using System;
using System.Globalization;
using System.Linq;

namespace RoumenBot
{
    public sealed class RoumenResponseLogger<T> : IRoumenResponseLogger<T> where T : ITag, new()
    {
        readonly TimeSpan MaxAge = TimeSpan.FromDays(7);

        private readonly IStorage<string> storage;

        public RoumenResponseLogger(IStorage<string> storage)
        {
            this.storage = storage;
        }

        public void LogResponse(string response)
        {
            var datetime = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            storage.Write(datetime, response);

            foreach (var key in storage.Iterate().Select(kvp => kvp.Key))
            {
                if (DateTimeOffset.TryParse(key, CultureInfo.InvariantCulture, out var parsed) || parsed < DateTimeOffset.UtcNow - MaxAge)
                {
                    storage.Delete(key);
                }
            }
        }
    }
}