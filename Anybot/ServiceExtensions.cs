
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Anybot.Commands;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly;
using System;

namespace Anybot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection WithAnybot(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<AnybotOptions>().Bind(context.Configuration.GetSection("Anybot"));
            services.AddHttpClient<ITelegramBotClient>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddSingleton<ITelegramBotClient>(services => new TelegramBotClient(services.GetService<IOptions<AnybotOptions>>().Value.Token, services.GetService<HttpClient>()));
            services.AddSingleton<ICommand, ChatIdCommand>();
            services.AddSingleton(s => s.GetServices<ICommand>().ToArray());
            services.AddHostedService<AnybotService>();

            return services;
        }
    }
}
