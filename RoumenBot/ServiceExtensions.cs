using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Polly.Extensions.Http;
using Polly;
using System;
using Anybot.Common;
using RocksDbSharp;

namespace RoumenBot
{
    public static class ServiceExtensions
    {
        private const string DbPrefix = "roumen";

        public static IServiceCollection WithRoumen(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<RoumenOptions>().Bind(context.Configuration.GetSection("Roumen"));
            services.AddSingleton<IRoumenParser, RoumenParser>();
            services.AddHttpClient<IRoumenRestService, RoumenRestService>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddHostedService<RoumenService>();
            services.AddSingleton<IRocksWrapper<RoumenImage>>(s => new RocksWrapper<RoumenImage>(s.GetService<RocksDb>(), DbPrefix));

            return services;
        }
    }
}
