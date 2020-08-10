
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Polly.Extensions.Http;
using Polly;
using System;
using Anybot.Common;
using RocksDbSharp;

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
            services.AddSingleton<IRocksWrapper<Partner>>(s => new RocksWrapper<Partner>(s.GetService<RocksDb>(), DbPrefix));

            return services;
        }
    }
}
