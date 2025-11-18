namespace GenAI.API.Models.Gemini
{
    public class GeminiRequest
    {
        public List<Content> Contents { get; set; }
        public GenerationConfig GenerationConfig { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }

    public class GenerationConfig
    {
        public ThinkingConfig ThinkingConfig { get; set; }
    }

    public class ThinkingConfig
    {
        public int ThinkingBudget { get; set; }
    }

}
