using GenAI.API.Enums;

namespace GenAI.API.Integrations.ApiKey.Models
{
    public class ConfigResponse
    {
        public Guid Id { get; set; }
        public string ApiKey { get; set; } = string.Empty;
        public KeyType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
