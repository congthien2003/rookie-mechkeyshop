using Shared.ViewModels.Abstractions;

namespace Shared.ViewModels.Product
{
    public class ProductModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ProductRatingModel> Rating { get; set; } = Enumerable.Empty<ProductRatingModel>();
        public double TotalRating { get; set; }
        public long SellCount { get; set; }
        public int Stock { get; set; }
        public List<VariantAttribute> Variants { get; set; } = new List<VariantAttribute>();
    }
}
