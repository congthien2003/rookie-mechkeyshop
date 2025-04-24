namespace Shared.ViewModels.ImageUpload
{
    public class ProductImageModel
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string Url { get; set; }
        public Guid ProductId { get; set; }
    }
}
