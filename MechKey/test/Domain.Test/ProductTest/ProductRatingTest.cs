using Domain.Entity;

namespace Domain.Test.ProductTest
{
    public class ProductRatingTest
    {
        private const string ProductName = "Test name";
        private const double ProductPrice = 99;
        private const string ProductDescription = "Test description";
        private const string ProductImageUrl = "url";

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void Add_Rating_Product(int rating, double avg_rating)
        {
            // arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Category" };
            var product = new Entity.Product
            {
                Id = Guid.NewGuid(),
                Name = ProductName,
                Price = ProductPrice,
                Description = ProductDescription,
                ImageUrl = ProductImageUrl,
                CategoryId = category.Id,
                Category = category,
                ProductRatings = new List<ProductRating>(),
                SellCount = 0,
                Variants = "",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var productRatings = new ProductRating
            {
                Id = 1,
                Product = product,
                ProductId = product.Id,
                Stars = rating,
                Comment = "comment 1",
                UserId = Guid.NewGuid(),
            };

            // act
            product.AddRating(productRatings);

            // assert
            Assert.Equal(product.AverageRating, avg_rating);
        }

        [Fact]
        public void Add_Multiple_Ratings_Product()
        {
            // arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Category" };
            var product = new Entity.Product
            {
                Id = Guid.NewGuid(),
                Name = ProductName,
                Price = ProductPrice,
                Description = ProductDescription,
                ImageUrl = ProductImageUrl,
                CategoryId = category.Id,
                Category = category,
                ProductRatings = new List<ProductRating>(),
                SellCount = 0,
                Variants = "",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var ratings = new List<int> { 4, 5, 3, 2, 5 }; // danh sách số sao
            foreach (var star in ratings)
            {
                var rating = new ProductRating
                {
                    Id = new Random().Next(1, 1000), // tạo random Id
                    Product = product,
                    ProductId = product.Id,
                    Stars = star,
                    Comment = "comment",
                    UserId = Guid.NewGuid(),
                };
                product.AddRating(rating);
            }

            // act
            var expectedAverage = ratings.Average(); // tính trung bình
            var actualAverage = product.AverageRating;

            // assert
            Assert.Equal(expectedAverage, actualAverage, precision: 2); // precision 2 chữ số sau dấu phẩy
        }

    }
}
