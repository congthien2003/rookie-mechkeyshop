var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddRabbitMQ("amqp://guest:guest@localhost:5672/", name: "rabbitmq");

var app = builder.Build();

app.MapGet("/health", async (HttpContext context) =>
{
    var report = await app.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();
    return Results.Json(new
    {
        status = report.Status.ToString(),
        results = report.Entries.Select(e => new
        {
            key = e.Key,
            status = e.Value.Status.ToString()
        })
    });
});