using Anybot.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using RocksDbSharp;
using System;

namespace RoumenBot
{
    public static class ServiceExtensions
    {
        private const string DbPrefix = "roumen";
        private const string DbPrefixMaso = "roumen_maso";

        public static IServiceCollection WithRoumen(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<RoumenOptions<Tag.Main>>().Bind(context.Configuration.GetSection("Roumen"));
            services.AddOptions<RoumenOptions<Tag.Maso>>().Bind(context.Configuration.GetSection("RoumenMaso"));
            services.AddSingleton<IRoumenParser, RoumenParser>();
            services.AddHttpClient<IRoumenRestService<Tag.Main>, RoumenRestService<Tag.Main>>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddHttpClient<IRoumenRestService<Tag.Maso>, RoumenRestService<Tag.Maso>>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddHostedService<RoumenService<Tag.Main>>();
            services.AddHostedService<RoumenService<Tag.Maso>>();
            services.AddSingleton<IRocksWrapper<RoumenImage<Tag.Main>>>(s => new RocksWrapper<RoumenImage<Tag.Main>>(s.GetService<RocksDb>(), DbPrefix));
            services.AddSingleton<IRocksWrapper<RoumenImage<Tag.Maso>>>(s => new RocksWrapper<RoumenImage<Tag.Maso>>(s.GetService<RocksDb>(), DbPrefixMaso));

            return services;
        }
    }
}
