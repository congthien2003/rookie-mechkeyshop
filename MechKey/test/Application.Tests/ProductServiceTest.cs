using Application.Comoon;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Mapping;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.Test
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository<Product>> _mockRepository;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly Mock<IProductUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ISupabaseService> _mockSupabaseClient;
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly ProductService _productService;
        private readonly CancellationToken _cancellationToken;
        private readonly Mock<IProductRatingRepository<ProductRating>> _mockProductRatingRepository;
        private readonly Mock<IProductMapping> _mockProductMapping;
        public ProductServiceTest()
        {
            _mockRepository = new Mock<IProductRepository<Product>>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockUnitOfWork = new Mock<IProductUnitOfWork>();
            _mockEventBus = new Mock<IEventBus>();
            _mockSupabaseClient = new Mock<ISupabaseService>();
            _mockRedisService = new Mock<IRedisService>();
            _mockProductMapping = new Mock<IProductMapping>();
            _mockProductRatingRepository = new Mock<IProductRatingRepository<ProductRating>>();
            _productService = new ProductService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockUnitOfWork.Object,
                _mockSupabaseClient.Object,
                _mockEventBus.Object,
                _mockRedisService.Object,
                _mockProductRatingRepository.Object,
                _mockProductMapping.Object
            );
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductSuccessfully_WithImageUpload()
        {
            // Arrange
            var createProductModel = new CreateProductModel
            {
                Name = "Test Product",
                CategoryId = Guid.NewGuid(),
                Price = 399,
                Description = "Test Description",
                Variants = [],
                ImageData = ""
            };

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 99,
                Description = "",
                CategoryId = Guid.NewGuid(),
                ImageUrl = "https://example.com/image.jpg",
                SellCount = 0
            };

            var productModel = ProductMapping.ToProductModel(product);

            var uploadFileResponse = new UploadFileResponseModel { FileName = "test", FilePath = "/test", PublicUrl = "https://example.com/image.jpg" };

            var resultUpload = Result<UploadFileResponseModel>.Success("Upload sucess", uploadFileResponse);
            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);

            var productImage = new ProductImage { FilePath = "/test", Url = "/test", ProductId = product.Id };

            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes)).ReturnsAsync(resultUpload);
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(_cancellationToken));
            _mockUnitOfWork.Setup(u => u.ProductRepository.CreateAsync(It.IsAny<Product>(), _cancellationToken)).ReturnsAsync(product);
            _mockUnitOfWork.Setup(u => u.CommitAsync(_cancellationToken));
            // Act
            var result = await _productService.AddAsync(createProductModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Product", result.Data.Name);
            _mockSupabaseClient.Verify(s => s.UploadImage(imageBytes), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(_cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenImageUploadFails()
        {
            // Arrange
            var createProductModel = new CreateProductModel
            {
                Name = "Test Product",
                Price = 30,
                CategoryId = Guid.NewGuid(),
                ImageData = ""
            };

            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);

            // Simulate image upload failure
            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes))
                .ThrowsAsync(new ProductImageHandleFailedException());

            // Act
            var ex = await Assert.ThrowsAsync<ProductImageHandleFailedException>(() => _productService.AddAsync(createProductModel));

            // Assert
            _mockSupabaseClient.Verify(s => s.UploadImage(imageBytes), Times.Once);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithInvalidName_ShouldThrowException()
        {
            // Arrange
            var invalidModel = new CreateProductModel
            {
                Name = "",
                CategoryId = Guid.NewGuid(),
                Price = 399,
                Description = "Test Description",
                Variants = [],
                ImageData = ""
            };

            // Act
            var exception = await Assert.ThrowsAsync<ProductInvalidDataException>(() => _productService.AddAsync(invalidModel));

            // Assert
            Assert.Equal("Product's name is required", exception.Message);

            // Không gọi upload ảnh vì dừng từ validation
            _mockSupabaseClient.Verify(x => x.UploadImage(It.IsAny<byte[]>()), Times.Never);

            // Không gọi lưu DB
            _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithInvalidImageData_ShouldThrowException()
        {
            // Arrange
            var invalidModel = new CreateProductModel
            {
                Name = "Test 1",
                CategoryId = Guid.NewGuid(),
                Price = 399,
                Description = "Test Description",
                Variants = [],
                ImageData = ""
            };

            // Act
            var exception = await Assert.ThrowsAsync<ProductInvalidDataException>(() => _productService.AddAsync(invalidModel));

            // Assert
            Assert.Equal("Product's image data is required", exception.Message);

            // Không gọi upload ảnh vì dừng từ validation
            _mockSupabaseClient.Verify(x => x.UploadImage(It.IsAny<byte[]>()), Times.Never);

            // Không gọi lưu DB
            _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithInvalidPrice_ShouldThrowException()
        {
            // Arrange
            var invalidModel = new CreateProductModel
            {
                Name = "Test",
                CategoryId = Guid.NewGuid(),
                Price = -1,
                Description = "Test Description",
                Variants = [],
                ImageData = ""
            };

            // Act
            var exception = await Assert.ThrowsAsync<ProductInvalidDataException>(() => _productService.AddAsync(invalidModel));

            // Assert
            Assert.Equal("Price must be positive", exception.Message);

            // Không gọi upload ảnh vì dừng từ validation
            _mockSupabaseClient.Verify(x => x.UploadImage(It.IsAny<byte[]>()), Times.Never);

            // Không gọi lưu DB
            _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Product>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProductSuccessfully_AndPublishEvent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.DeleteAsync(product, _cancellationToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Delete product success", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound_WithProperHandling()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync((Product?)null);

            // Act
            var ex = await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.DeleteAsync(productId));

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(productId, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedProducts()
        {
            // Arrange
            var pagiModel = new PaginationReqModel { Page = 1, PageSize = 10 };
            var category = new Category { Id = Guid.NewGuid(), Name = "Category Name" };
            var products = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Product 1", Category = category, CategoryId = category.Id },
                    new Product { Id = Guid.NewGuid(), Name = "Product 2", Category = category, CategoryId = category.Id }
                };

            var queryable = products.AsQueryable().BuildMock();

            _mockRedisService.Setup(r => r.Get<PagedResult<ProductModel>>("test")).Returns((PagedResult<ProductModel>)null);

            _mockRepository.Setup(r => r.GetAllAsync()).Returns(queryable);
            // Act
            var result = await _productService.GetAllAsync(pagiModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Items.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Name = "Fake User",
            };
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Category A" },
                ImageUrl = "http://image.url",
                Price = 100,
                SellCount = 10,
                IsDeleted = false,
                Variants = JsonConvert.SerializeObject(new List<VariantAttribute>
                {
                    new VariantAttribute { Name = "Size", Value = ["M"] }
                }),
                ProductRatings = new List<ProductRating>
                {
                    new ProductRating
                    {
                        Id = 1,
                        ProductId = productId,
                        UserId = Guid.NewGuid(),
                        User = user,
                        RatedAt = DateTime.UtcNow,
                        Comment = "Good",
                        Stars = 4
                    }
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(product.Name, result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId, _cancellationToken)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.GetByIdAsync(productId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProductSuccessfully()
        {
            // Arrange
            var updateProductModel = new UpdateProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Updated Product",
                Price = 300,
                CategoryId = Guid.NewGuid(),
            };
            var product = new Product { Id = updateProductModel.Id, Name = "Old Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(updateProductModel.Id, _cancellationToken)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.UpdateAsync(product, _cancellationToken)).ReturnsAsync(product);

            // Act
            var result = await _productService.UpdateAsync(updateProductModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Update product success", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenProductDoesNotExist()
        {
            // Arrange
            var updateProductModel = new UpdateProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Updated Product",
                Price = 300,
                CategoryId = Guid.NewGuid(),
            };

            _mockRepository.Setup(r => r.GetByIdAsync(updateProductModel.Id, _cancellationToken)).ReturnsAsync((Product)null);

            // Act
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.UpdateAsync(updateProductModel));

            // Assert
            Assert.Equal("Product not found", exception.Message);
        }
    }
}
