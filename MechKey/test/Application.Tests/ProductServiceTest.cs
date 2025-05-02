using Application.Comoon;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Common;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.xUnitTest
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository<Product>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly Mock<IProductUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ISupabaseService> _mockSupabaseClient;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _mockRepository = new Mock<IProductRepository<Product>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockUnitOfWork = new Mock<IProductUnitOfWork>();
            _mockEventBus = new Mock<IEventBus>();
            _mockSupabaseClient = new Mock<ISupabaseService>();
            _productService = new ProductService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockUnitOfWork.Object,
                _mockSupabaseClient.Object,
                _mockEventBus.Object
            );
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductSuccessfully_WithImageUpload()
        {
            // Arrange
            var createProductModel = new CreateProductModel
            {
                Name = "Test Product",
                ImageData = "base64string"
            };

            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", ImageUrl = "https://example.com/image.jpg" };

            var productModel = new ProductModel { Id = product.Id, Name = product.Name, ImageUrl = product.ImageUrl };

            var uploadFileResponse = new UploadFileResponseModel { FileName = "test", FilePath = "/test", PublicUrl = "https://example.com/image.jpg" };

            var resultUpload = Result<UploadFileResponseModel>.Success("Upload sucess", uploadFileResponse);
            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);


            _mockMapper.Setup(m => m.Map<Product>(createProductModel)).Returns(product);
            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes)).ReturnsAsync(resultUpload);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductModel>(product)).Returns(productModel);

            // Act
            var result = await _productService.AddAsync(createProductModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Add product success", result.Message);
            Assert.Equal(productModel, result.Data);
            _mockSupabaseClient.Verify(s => s.UploadImage(imageBytes), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenImageUploadFails_WithProperHandling()
        {
            // Arrange
            var createProductModel = new CreateProductModel { Name = "Test Product", ImageData = "base64string" };

            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);

            // Simulate image upload failure
            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes))
                .ThrowsAsync(new Exception("Image upload failed"));

            // Act & Assert
            await Assert.ThrowsAsync<ProductHandleFailedException>(() => _productService.AddAsync(createProductModel));
            _mockSupabaseClient.Verify(s => s.UploadImage(imageBytes), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProductSuccessfully_AndPublishEvent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Delete product success", result.Message);
            _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound_WithProperHandling()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.DeleteAsync(productId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.DeleteAsync(productId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedProducts()
        {
            // Arrange
            var pagiModel = new PaginationReqModel { Page = 1, PageSize = 10 };
            var products = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Product 1" },
                    new Product { Id = Guid.NewGuid(), Name = "Product 2" }
                };

            _mockRepository.Setup(r => r.GetAllAsync()).Returns(products.AsQueryable());

            // Act
            var result = await _productService.GetAllAsync(pagiModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Items.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };
            var productModel = new ProductModel { Id = productId, Name = "Test Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductModel>(product)).Returns(productModel);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(productModel, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _productService.GetByIdAsync(productId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProductSuccessfully()
        {
            // Arrange
            var updateProductModel = new UpdateProductModel { Id = Guid.NewGuid(), Name = "Updated Product" };
            var product = new Product { Id = updateProductModel.Id, Name = "Old Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(updateProductModel.Id)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.UpdateAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _productService.UpdateAsync(updateProductModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Update product success", result.Message);
        }
    }
}
