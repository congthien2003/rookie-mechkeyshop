namespace Shared.ViewModels.Product
{
    public class UpdateProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public List<VariantAttribute> Variants { get; set; } = new List<VariantAttribute>();
    }
}
