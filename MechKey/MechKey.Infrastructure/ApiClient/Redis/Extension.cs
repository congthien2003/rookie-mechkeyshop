using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Infrastructure.ApiClient.Redis
{
    public static class Extension
    {
        public static IHostApplicationBuilder AddRedisCache(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
            });

            return builder;
        }
    }
}
