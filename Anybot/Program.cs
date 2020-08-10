using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using RocksDbSharp;
using Anybot.Common;
using RoumenBot;
using ActivePass;
using Microsoft.VisualBasic;
using System;
using System.Linq;

namespace Anybot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .Build()
                .RunAsync().ConfigureAwait(false);
        }

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
        {
            if (!context.HostingEnvironment.IsDevelopment())
            {
                //                var syslogSettings = new SyslogLoggerSettings() { MessageTransportProtocol = Syslog.Framework.Logging.TransportProtocols.TransportProtocol.UnixSocket};
                //                builder.AddProvider(new SyslogLoggerProvider(syslogSettings, "localhost", LogLevel.Information));
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IDelayer>(services => new Delayer(services.GetService<IOptions<AnybotOptions>>().Value.PostDelay));
            services.WithRocksDb();
            services.WithRoumen(context);
            services.WithActivePass(context);
            services.WithAnybot(context);
        }

        private static IServiceCollection WithRocksDb(this IServiceCollection services)
        {
            var rocksDbOptions = new DbOptions().SetCreateIfMissing(true);

            return services.AddSingleton(s => RocksDb.Open(rocksDbOptions, s.GetService<IOptions<AnybotOptions>>().Value.Database));
        }
    }
}
