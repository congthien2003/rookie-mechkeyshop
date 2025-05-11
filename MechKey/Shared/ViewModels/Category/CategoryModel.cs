using Shared.ViewModels.Abstractions;

namespace Shared.ViewModels.Category
{

    public class CategoryModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
