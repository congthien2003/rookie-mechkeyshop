using Domain.Entity;
using Domain.Exceptions;

namespace Domain.Test.ProductTest
{
    public class ProductIncreaseSellCountTest
    {
        private const string ProductName = "Test name";
        private const double ProductPrice = 99;
        private const string ProductDescription = "Test description";
        private const string ProductImageUrl = "url";

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 100)]
        public void Add_Increase_Sell_Count_Product(int quantity, int total)
        {
            // arrange
            var category = new Category(Guid.NewGuid(), "Category");
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

            // act
            product.IncreaseSellCount(quantity);

            // assert
            Assert.Equal(product.SellCount, total);
        }

        [Fact]
        public void Add_Increase_Sell_Count_Product_With_Negative_Number_Should_Throw_Exception()
        {
            // arrange
            var category = new Category(Guid.NewGuid(), "Category");
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

            // assert
            Assert.Throws<ProductValidateFailedException>(() => product.IncreaseSellCount(-1));
        }
    }
}
