using Infrastructure.ApiClient.MassTransit.Consumer;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.ApiClient.MassTransit
{
    public static class Extension
    {
        public static IHostApplicationBuilder AddMassTransit(this IHostApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<RegisterSuccessConsumer>();
                busConfigurator.AddConsumer<OrderCreatedConsumer>();
                busConfigurator.AddConsumer<DeleteImageConsumer>();


                busConfigurator.UsingRabbitMq((context, config) =>
                {

                    config.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    config.ReceiveEndpoint("email", e =>
                    {
                        e.ConfigureConsumer<RegisterSuccessConsumer>(context);
                    });

                    config.ReceiveEndpoint("order", e =>
                    {
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    });

                    config.ReceiveEndpoint("delete-image", e =>
                    {
                        e.ConfigureConsumer<DeleteImageConsumer>(context);
                    });

                });

            });

            return builder;
        }
    }
}
