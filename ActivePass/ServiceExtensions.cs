
using Anybot.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using RocksDbSharp;
using System;

namespace ActivePass
{
    public static class ServiceExtensions
    {
        private const string DbPrefix = "activepass";

        public static IServiceCollection WithActivePass(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<BotOptions>().Bind(context.Configuration.GetSection("Activepass"));
            services.AddHttpClient<IActivePassRestService, ActivePassRestService>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddHostedService<ActivePassService>();
            services.AddSingleton<IRocksWrapper<Partner>>(s => new RocksWrapper<Partner>(s.GetRequiredService<RocksDb>(), DbPrefix));

            return services;
        }
    }
}
