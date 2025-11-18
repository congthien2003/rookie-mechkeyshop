using GenAI.API.Models.Generate;

namespace GenAI.API.Services.Interfaces
{
    public interface IGenAIService
    {
        Task<GenerateResponse> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
