using Domain.Common;
using Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public string ImageUrl { get; set; } = "";
        [Required(ErrorMessage = "CategoryId is required")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();
        public string Variants { get; set; } = string.Empty;
        public long SellCount { get; set; } = 0;


        /// <summary>
        /// Add rating to the product, you can add duplicate check here
        /// </summary>
        public void AddRating(ProductRating rating)
        {
            if (rating == null)
                throw new ArgumentNullException(nameof(rating));

            // Optional: prevent user from rating multiple times
            var existing = ProductRatings.FirstOrDefault(r => r.UserId == rating.UserId);
            if (existing != null)
                throw new InvalidOperationException("User has already rated this product.");

            // Add rating to collection
            ProductRatings.Add(rating);
        }

        public double AverageRating => ProductRatings.Any()
            ? Math.Round(ProductRatings.Average(r => r.Stars), 2)
            : 0;

        public void IncreaseSellCount(int quantity)
        {
            if (quantity < 1) throw new ProductValidateFailedException();
            SellCount += quantity;
        }
    }
}
