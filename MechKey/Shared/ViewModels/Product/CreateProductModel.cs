namespace Shared.ViewModels.Product
{
    public class CreateProductModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Guid CategoryId { get; set; }
        public List<VariantAttribute> Variants { get; set; } = new List<VariantAttribute>();
        public string ImageData { get; set; }
    }
}
