using GenAI.API.Models.Gemini;
using GenAI.API.Models.Generate;
using GenAI.API.Services.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;
namespace GenAI.API.Services.Implementations
{
    public class GenAIService : IGenAIService
    {
        private readonly ILogger<GenAIService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string Url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public GenAIService(IHttpClientFactory httpClientFactory, ILogger<GenAIService> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("GeminiClient");
        }

        public async Task<GenerateResponse> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            try
            {

                _logger.LogInformation("Get Key in Cloud Database");
                //var _geminiConfig = await _configRepository.GetKeyAsync(cancellationToken);

                var request = new GeminiRequest
                {
                    Contents = new List<Content>
                    {
                        new Content
                        {
                            Parts = new List<Part>
                            {
                                new Part { Text = prompt }
                            }
                        }
                    },
                    GenerationConfig = new GenerationConfig
                    {
                        ThinkingConfig = new ThinkingConfig
                        {
                            ThinkingBudget = 0
                        }
                    }
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var apiKey = "AIzaSyC5t5auj74nLl5l-ziV88QobrX2-LINInE";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Add("X-goog-api-key", apiKey);

                var response = await _httpClient.PostAsync(Url, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();

                // Parse response
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var output = geminiResponse?.Candidates?.FirstOrDefault()
                                           ?.Content?.Parts?.FirstOrDefault()
                                           ?.Text;

                var result = new GenerateResponse
                {
                    IsSuccess = true,
                    Result = output ?? string.Empty,
                    ErrorCode = null,
                    CreatedAt = DateTime.Now
                };

                return result;
            }

            catch (Exception ex)
            {
                var result = new GenerateResponse
                {
                    IsSuccess = false,
                    Result = string.Empty,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.Now
                };

                return result;
            }
        }
    }
}
