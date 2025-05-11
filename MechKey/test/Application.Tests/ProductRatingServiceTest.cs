using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Test
{
    public class ProductRatingServiceTest
    {
        private readonly Mock<IProductRepository<Product>> _productRepositoryMock;
        private readonly Mock<IProductRatingRepository<ProductRating>> _productRatingRepositoryMock;
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _applicationUserRepositoryMock;
        private readonly Mock<ILogger<ProductRatingService>> _loggerMock;
        private readonly Mock<IProductRatingMapping> _productRatingMappingMock;
        private readonly ProductRatingService _productRatingService;
        private readonly CancellationToken _cancellationToken;

        public ProductRatingServiceTest()
        {
            _productRepositoryMock = new Mock<IProductRepository<Product>>();
            _productRatingRepositoryMock = new Mock<IProductRatingRepository<ProductRating>>();
            _applicationUserRepositoryMock = new Mock<IApplicationUserRepository<ApplicationUser>>();
            _loggerMock = new Mock<ILogger<ProductRatingService>>();
            _productRatingMappingMock = new Mock<IProductRatingMapping>();

            _productRatingService = new ProductRatingService(
                _productRatingRepositoryMock.Object,
                _loggerMock.Object,
                _applicationUserRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productRatingMappingMock.Object
            );
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task AddAsync_ValidRating_ReturnsSuccess()
        {
            // Arrange
            var ratingModel = new ProductRatingModel
            {
                Id = 1,
                Stars = 5,
                Comment = "Great product!",
                RatedAt = DateTime.UtcNow,
                ProductId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var rating = new ProductRating
            {
                Id = ratingModel.Id,
                Stars = ratingModel.Stars,
                Comment = ratingModel.Comment,
                RatedAt = ratingModel.RatedAt,
                ProductId = ratingModel.ProductId,
                UserId = ratingModel.UserId
            };

            var product = new Product
            {
                Id = ratingModel.ProductId,
                Name = "Test Product",
                Price = 100
            };

            _productRatingMappingMock.Setup(x => x.ToProductRating(It.IsAny<ProductRatingModel>()))
                .Returns(rating);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(ratingModel.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _productRatingRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<ProductRating>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rating);

            // Act
            var result = await _productRatingService.AddAsync(ratingModel);

            // Assert
            Assert.True(result.IsSuccess);
            _productRatingRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<ProductRating>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenProductNotFound()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };
            var ratingModel = new ProductRatingModel
            {
                ProductId = productId,
                UserId = Guid.NewGuid(),
                Stars = 5,
                Comment = "Great product!",
                Name = "Name test",
            };
            var productRating = new ProductRating
            {
                Id = 2,
                ProductId = productId,
                UserId = ratingModel.UserId,
                Stars = 5,
                Comment = "Great product!",
                RatedAt = DateTime.UtcNow
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync((Product)null);

            // Act
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _productRatingService.AddAsync(ratingModel));

            // Assert
            Assert.Equal("Product not found", exception.Message);
            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenInvalidScore()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };
            var ratingModel = new ProductRatingModel
            {
                ProductId = productId,
                UserId = Guid.NewGuid(),
                Stars = -1,
                Comment = "Great product!"
            };
            var productRating = new ProductRating
            {
                ProductId = productId,
                UserId = ratingModel.UserId,
                Stars = ratingModel.Stars,
                Comment = ratingModel.Comment,
                RatedAt = DateTime.UtcNow
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync(product);

            // Act
            var exception = await Assert.ThrowsAsync<ProductRatingInvalidDataException>(() => _productRatingService.AddAsync(ratingModel));

            // Assert
            Assert.Equal("Stars must be between 1 and 5", exception.Message);
            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task GetAllByIdProductAsync_ReturnsPagedRatings()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var pageSize = 4;
            var ratings = new List<ProductRating>
            {
                new ProductRating
                {
                    Id = 1,
                    Stars = 5,
                    Comment = "Great product!",
                    RatedAt = DateTime.UtcNow,
                    ProductId = productId,
                    UserId = Guid.NewGuid(),
                    User = new ApplicationUser { Name = "Test User" }
                }
            };

            var ratingModels = new List<ProductRatingModel>
            {
                new ProductRatingModel
                {
                    Id = ratings[0].Id,
                    Stars = ratings[0].Stars,
                    Comment = ratings[0].Comment,
                    RatedAt = ratings[0].RatedAt,
                    ProductId = ratings[0].ProductId,
                    UserId = ratings[0].UserId,
                    Name = ratings[0].User.Name
                }
            };

            _productRatingRepositoryMock.Setup(x => x.GetListByProduct(productId))
                .Returns(ratings.AsQueryable());

            _productRatingMappingMock.Setup(x => x.ToProductRatingModel(It.IsAny<ProductRating>()))
                .Returns((ProductRating r) => ratingModels.First(m => m.Id == r.Id));

            // Act
            var result = await _productRatingService.GetAllByIdProductAsync(productId, pageSize);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ratingModels, result.Data.Items);
            _productRatingRepositoryMock.Verify(x => x.GetListByProduct(productId), Times.Once);
            _productRatingMappingMock.Verify(x => x.ToProductRatingModel(It.IsAny<ProductRating>()), Times.Once);
        }
    }
}
