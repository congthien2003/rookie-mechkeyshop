using Domain.Common;

namespace Domain.Entity
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();

    }
}
