namespace Shared.ViewModels
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
