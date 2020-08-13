using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Anybot.Common;
using RoumenBot;
using ActivePass;

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
                .Build()
                .RunAsync().ConfigureAwait(false);
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IDelayer>(services => new Delayer(services.GetService<IOptions<AnybotOptions>>().Value.PostDelay));
            services.WithRoumen(context);
            services.WithActivePass(context);
            services.WithAnybot(context);
        }
    }
}
