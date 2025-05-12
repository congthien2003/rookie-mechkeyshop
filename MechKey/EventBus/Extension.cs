using Microsoft.Extensions.DependencyInjection;

namespace EventBus
{
    public static class ExtensionEventBus
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddScoped<IEventBus, Implementation.EventBus>();

            return services;
        }
    }
}
