using Anybot.Commands;
using Anybot.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Linq;
using System.Net.Http;
using Telegram.Bot;

namespace Anybot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection WithAnybot(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<AnybotOptions>().Bind(context.Configuration.GetSection("Anybot"));

            services.AddHttpClient<ITelegramBotClient>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddSingleton<ITelegramBotClient>(services => new TelegramBotClient(services.GetRequiredService<IOptions<AnybotOptions>>().Value.Token!, services.GetService<HttpClient>()));
            services.AddSingleton<ICommand, ChatIdCommand>();
            services.AddSingleton(s => s.GetServices<ICommand>().ToArray());
            services.AddSingleton(s => new FsdbProvider(s.GetRequiredService<IOptions<AnybotOptions>>().Value.Database));
            services.AddHostedService<AnybotService>();

            return services;
        }
    }
}
