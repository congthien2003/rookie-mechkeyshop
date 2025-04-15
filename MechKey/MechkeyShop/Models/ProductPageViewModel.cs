using Shared.ViewModels;

namespace MechkeyShop.Models
{
    public class ProductPageViewModel
    {
        public IEnumerable<CategoryModel> categories { get; set; }
        public IEnumerable<ProductModel> products { get; set; }
        public string categoryId { get; set; } = string.Empty;
        public string searchTerm { get; set; } = string.Empty;
        public string sortCol { get; set; } = "";
        public int ascendingOrder { get; set; } = 1;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int totalCount { get; set; } = 0;
        public int totalPages { get; set; } = 0;
    }
}
