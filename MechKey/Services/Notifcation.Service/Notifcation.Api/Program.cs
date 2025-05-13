using EventBus;
using MassTransit;
using Notifcation.Api;
using Notifcation.Infrastructure;
using Notification.Application.Common;
using Notification.Application.Interfaces;
using Notification.Application.Services;
using Notification.Consumer;
using Notification.Domain.IRepository;
using Notification.Infrastructure.Mappers;
using Notification.Infrastructure.MongoDb.DataContext;
using Notification.Infrastructure.MongoDb.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

builder.Services.AddEventBus();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationMapper, NotificationMapper>();

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

app.MapNotificationApi();

app.Run();
