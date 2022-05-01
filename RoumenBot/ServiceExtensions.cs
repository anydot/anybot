using Anybot.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using RoumenBot.Tag;
using System;

namespace RoumenBot
{
    public static class ServiceExtensions
    {
        private const string DbPrefix = "roumen";
        private const string DbPrefixMaso = "roumen_maso";

        public static IServiceCollection WithRoumen(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddOptions<RoumenOptions<Main>>().Bind(context.Configuration.GetSection("Roumen"));
            services.AddOptions<RoumenOptions<Maso>>().Bind(context.Configuration.GetSection("RoumenMaso"));
            services.AddSingleton<IRoumenParser, RoumenParser>();
            services.AddHttpClient<IRoumenRestService<Main>, RoumenRestService<Main>>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddHttpClient<IRoumenRestService<Maso>, RoumenRestService<Maso>>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddHostedService<RoumenService<Main>>();
            services.AddHostedService<RoumenService<Maso>>();
            services.AddSingleton(s => s.GetRequiredService<FsdbProvider>().Create<RoumenImage<Main>>(DbPrefix));
            services.AddSingleton(s => s.GetRequiredService<FsdbProvider>().Create<RoumenImage<Maso>>(DbPrefixMaso));

            return services;
        }
    }
}
