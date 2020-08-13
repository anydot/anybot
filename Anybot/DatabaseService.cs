using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System.Threading;
using System.Threading.Tasks;

namespace Anybot
{
    public class DatabaseService : BackgroundService
    {
        private readonly IOptions<AnybotOptions> options;
        private readonly RocksDb rocksDb;
        private readonly ILogger<DatabaseService> logger;

        public DatabaseService(IOptions<AnybotOptions> options, RocksDb rocksDb, ILogger<DatabaseService> logger)
        {
            this.options = options;
            this.rocksDb = rocksDb;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogDebug("Compacting DB");
                rocksDb.CompactRange((byte[]?)null, null);
                logger.LogDebug("Compaction done");

                await Task.Delay(options.Value.DatabaseCompact, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}