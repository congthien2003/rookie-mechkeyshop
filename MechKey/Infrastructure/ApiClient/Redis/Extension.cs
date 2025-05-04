using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.ApiClient.Redis
{
    public static class Extension
    {
        public static IHostApplicationBuilder AddRedisCache(this IHostApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
                options.InstanceName = "MechkeyShop-Redis";
            });

            return builder;
        }
    }
}
