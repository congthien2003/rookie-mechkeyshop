using Shared.ViewModels;

namespace MechkeyShop.Models
{
    public class ProductPageViewModel
    {
        public IEnumerable<CategoryModel> Categories { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;
        public string SortCol { get; set; } = "";
        public int AscendingOrder { get; set; } = 1;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
    }
}
