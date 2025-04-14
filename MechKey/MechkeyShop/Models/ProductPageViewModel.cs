using Shared.ViewModels;

namespace MechkeyShop.Models
{
    public class ProductPageViewModel
    {
        public IEnumerable<CategoryModel> Categories { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
        public string categoryId { get; set; } = string.Empty;
        public string searchTerm { get; set; } = string.Empty;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int totalCount { get; set; } = 0;
        public int totalPages { get; set; } = 0;
    }
}
