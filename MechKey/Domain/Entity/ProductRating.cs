using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class ProductRating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; }
        public string Comment { get; set; } = "";
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
        // Foreign key
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
