using ActivePass;
using Anybot.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RoumenBot;
using System.Threading.Tasks;

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
