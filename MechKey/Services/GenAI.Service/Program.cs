using GenAI.API.Endpoints;
using GenAI.API.Models.Gemini;
using GenAI.API.Services.Implementations;
using GenAI.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.AddServiceDefaults();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<IGenAIService, GenAIService>(client =>
{
    var geminiConfig = builder.Configuration.GetSection("GeminiConfig").Get<GeminiConfig>();
    client.BaseAddress = new Uri(geminiConfig!.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(120);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-goog-api-key", $"{geminiConfig.ApiKey}");
});

builder.Services.AddScoped<IGenAIService, GenAIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGenAiEndpoints();

app.MapControllers();

app.Run();
