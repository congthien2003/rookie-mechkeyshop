namespace GenAI.API.Models.Generate
{
    public class GenerateResponse
    {
        public bool IsSuccess { get; set; }
        public string Result { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public int? ErrorCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
