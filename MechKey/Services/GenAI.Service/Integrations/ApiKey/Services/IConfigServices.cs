using GenAI.API.Integrations.ApiKey.Models;

namespace GenAI.API.Integrations.ApiKey.Services
{
    public interface IConfigServices
    {
        public Task<ConfigResponse> GetGeminiApiKeyAsync();
    }
}
