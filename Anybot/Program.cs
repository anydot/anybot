using Anybot.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RoumenBot;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace Anybot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Debug.Assert(!OperatingSystem.IsAndroid());
            Debug.Assert(!OperatingSystem.IsIOS());

            await Host
                .CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices(ConfigureServices)
                .Build()
                .RunAsync().ConfigureAwait(false);
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IDelayer>(services => new Delayer(services.GetRequiredService<IOptions<AnybotOptions>>().Value.PostDelay));
            services.WithRoumen(context);
            services.WithAnybot(context);
        }
    }
}
