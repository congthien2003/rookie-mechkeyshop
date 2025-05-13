using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Common;
using Order.Application.Interfaces;
using Order.Infrastructure.Comsumers;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.ReadOrderService;
namespace Order.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IEventStore>(sp => new PostgresEventStore(config.GetConnectionString("DefaultConnection")!));
            services.AddSingleton<IMongoDbContext, ReadDbContext.QueryDbContext>();

            services.AddScoped<IReadOrderService, Infrastructure.ReadOrderService.ReadOrderService>();
            services.AddScoped<IReadOrderWriter, ReadOrderWriter>();

            services.AddMassTransit(
                busConfigurator =>
                {
                    busConfigurator.SetKebabCaseEndpointNameFormatter();
                    busConfigurator.AddConsumer<OrderCreatedConsumer>();
                    busConfigurator.UsingRabbitMq((context, config) =>
                    {

                        config.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        config.ReceiveEndpoint("order-created", e =>
                        {
                            e.ConfigureConsumer<OrderCreatedConsumer>(context);
                        });

                    });

                });
            return services;
        }
    }
}
