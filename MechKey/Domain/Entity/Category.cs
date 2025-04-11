using Domain.Common;

namespace Domain.Entity
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; }
    }
}