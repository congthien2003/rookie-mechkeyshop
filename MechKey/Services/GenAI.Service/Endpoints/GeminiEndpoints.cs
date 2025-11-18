using GenAI.API.Models.Generate;
using GenAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenAI.API.Endpoints
{
    public static class GeminiEndpoints
    {
        public static void MapGenAiEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/gemini/gen", async ([FromBody] GenerateRequest request, IGenAIService aiService) =>
            {
                var result = await aiService.GenerateAsync(request.Prompt);
                return Results.Ok(result);
            })
            .WithName("GenerateText")
            .WithOpenApi();
        }
    }
}
