using Application.Services;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Shared.ViewModels.Product;

namespace Application.Test
{
    public class ProductRatingServiceTest
    {
        private readonly Mock<IProductRepository<Product>> _mockProductRepository;
        private readonly Mock<IProductRatingRepository<ProductRating>> _mockProductRatingRepository;
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _mockApplicationUserRepository;
        private readonly Mock<ILogger<ProductRatingService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductRatingService _productRatingService;
        private readonly CancellationToken _cancellationToken;

        public ProductRatingServiceTest()
        {
            _mockProductRepository = new Mock<IProductRepository<Product>>();
            _mockProductRatingRepository = new Mock<IProductRatingRepository<ProductRating>>();
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository<ApplicationUser>>();
            _mockLogger = new Mock<ILogger<ProductRatingService>>();
            _mockMapper = new Mock<IMapper>();

            _productRatingService = new ProductRatingService(
                _mockProductRatingRepository.Object,
                _mockLogger.Object,
                _mockMapper.Object,
                _mockApplicationUserRepository.Object,
                _mockProductRepository.Object
            );
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task AddAsync_ShouldAddRatingSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };
            var ratingModel = new ProductRatingModel
            {
                ProductId = productId,
                UserId = Guid.NewGuid(),
                Stars = 5,
                Comment = "Great product!",
                Name = "Test name",
            };
            var productRating = new ProductRating
            {
                Id = 1,
                ProductId = productId,
                UserId = ratingModel.UserId,
                Stars = ratingModel.Stars,
                Comment = ratingModel.Comment,
                RatedAt = DateTime.UtcNow
            };

            _mockMapper.Setup(m => m.Map<ProductRating>(ratingModel)).Returns(productRating);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync(product);
            _mockProductRatingRepository.Setup(r => r.CreateAsync(It.IsAny<ProductRating>(), _cancellationToken)).ReturnsAsync(productRating);

            // Act
            var result = await _productRatingService.AddAsync(ratingModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Add rating success", result.Message);
            _mockProductRatingRepository.Verify(r => r.CreateAsync(It.IsAny<ProductRating>(), _cancellationToken), Times.Once);
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

            _mockMapper.Setup(m => m.Map<ProductRating>(ratingModel)).Returns(productRating);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync((Product)null);

            // Act
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _productRatingService.AddAsync(ratingModel));

            // Assert
            Assert.Equal("Product not found", exception.Message);
            _mockProductRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);

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

            _mockMapper.Setup(m => m.Map<ProductRating>(ratingModel)).Returns(productRating);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync(product);

            // Act
            var exception = await Assert.ThrowsAsync<ProductRatingInvalidDataException>(() => _productRatingService.AddAsync(ratingModel));

            // Assert
            Assert.Equal("Stars must be between 1 and 5", exception.Message);
            _mockProductRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);

        }

        [Fact]
        public async Task GetAllByIdProductAsync_ShouldReturnPaginatedRatings()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var ratings = new List<ProductRating>
            {
                new ProductRating { Id = 1, ProductId = productId, Stars = 5, Comment = "Excellent!" },
                new ProductRating { Id = 2, ProductId = productId, Stars = 4, Comment = "Good!" }
            };
            var ratingModels = ratings.Select(r => new ProductRatingModel
            {
                ProductId = r.ProductId,
                Stars = r.Stars,
                Comment = r.Comment
            }).ToList();

            var queryable = ratings.AsQueryable().BuildMock();

            _mockProductRatingRepository.Setup(r => r.GetListByProduct(productId)).Returns(queryable);

            _mockMapper.Setup(m => m.Map<ProductRatingModel>(It.IsAny<ProductRating>()))
                .Returns((ProductRating r) => new ProductRatingModel
                {
                    ProductId = r.ProductId,
                    Stars = r.Stars,
                    Comment = r.Comment
                });

            // Act
            var result = await _productRatingService.GetAllByIdProductAsync(productId, pageSize: 2, false, _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Get rating success", result.Message);
            Assert.Equal(2, result.Data.Items.Count());
            Assert.Equal(2, result.Data.TotalItems);
        }
    }
}
