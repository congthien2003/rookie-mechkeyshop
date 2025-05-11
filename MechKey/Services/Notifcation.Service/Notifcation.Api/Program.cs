using Constracts.Events;
using EventBus;
using MassTransit;
using Notifcation.Infrastructure;
using Notification.Application;
using Notification.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddScoped<IEventBus, EventBus.Implementation.EventBus>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<RegisterSuccessConsumer>();
    busConfigurator.AddConsumer<OrderCreatedConsumer>();


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

    });

});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/health-check", (IEventBus eventBus, CancellationToken token) =>
{
    eventBus.Publish(new RegisterSuccessEvent
    {
        CreatedAt = DateTime.Now,
        Email = "nhoccuthien0538@gmail.com",
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid()
    }, token);
    return Results.Ok("Health");
});

app.Run();
