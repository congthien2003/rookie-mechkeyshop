using Application.Comoon;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shared.Common;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.Test
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository<Product>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly Mock<IProductUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ISupabaseService> _mockSupabaseClient;
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _mockRepository = new Mock<IProductRepository<Product>>();
            _mockMapper = new Mock<IMapper>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductModel>()
                    .ForMember(p => p.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                    .ForMember(p => p.Rating, opt => opt.MapFrom(src => src.ProductRatings))
                    .ForMember(p => p.TotalRating,
                    opt => opt.MapFrom(src => src.ProductRatings.Count() > 0 ? src.ProductRatings.Average(pr => pr.Stars) : 0))
                    .ForMember(p => p.Variants, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<VariantAttribute>>(src.Variants)))
                    .ReverseMap();

                cfg.CreateMap<ProductRating, ProductRatingModel>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                    .ReverseMap();

                cfg.CreateMap<CreateProductModel, Product>()
                    .ForMember(p => p.Variants, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Variants)));
            });

            var mapper = config.CreateMapper();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockUnitOfWork = new Mock<IProductUnitOfWork>();
            _mockEventBus = new Mock<IEventBus>();
            _mockSupabaseClient = new Mock<ISupabaseService>();
            _mockRedisService = new Mock<IRedisService>();
            _productService = new ProductService(
                _mockRepository.Object,
                mapper,
                _mockLogger.Object,
                _mockUnitOfWork.Object,
                _mockSupabaseClient.Object,
                _mockEventBus.Object,
                _mockRedisService.Object
            );
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
                ImageData = "data:image/jpeg;base64,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"
            };

            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Price = 99, Description = "", CategoryId = Guid.NewGuid(), ImageUrl = "https://example.com/image.jpg" };

            var productModel = new ProductModel { Id = product.Id, Name = product.Name, ImageUrl = product.ImageUrl };

            var uploadFileResponse = new UploadFileResponseModel { FileName = "test", FilePath = "/test", PublicUrl = "https://example.com/image.jpg" };

            var resultUpload = Result<UploadFileResponseModel>.Success("Upload sucess", uploadFileResponse);
            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);

            var productImage = new ProductImage { FilePath = "/test", Url = "/test", ProductId = product.Id };


            _mockMapper.Setup(m => m.Map<Product>(createProductModel)).Returns(product);
            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes)).ReturnsAsync(resultUpload);
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync());
            _mockMapper.Setup(m => m.Map<ProductModel>(product)).Returns(productModel);
            _mockUnitOfWork.Setup(u => u.ProductImageRepository.CreateAsync(It.IsAny<ProductImage>()));
            _mockUnitOfWork.Setup(u => u.ProductRepository.CreateAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _mockUnitOfWork.Setup(u => u.CommitAsync());
            // Act
            var result = await _productService.AddAsync(createProductModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Product", result.Data.Name);
            _mockSupabaseClient.Verify(s => s.UploadImage(imageBytes), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);

        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenImageUploadFails_WithProperHandling()
        {
            // Arrange
            var createProductModel = new CreateProductModel { Name = "Test Product", ImageData = "data:image/jpeg;base64,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" };

            var img = createProductModel.ImageData.Substring(23);
            byte[] imageBytes = Convert.FromBase64String(img);

            // Simulate image upload failure
            _mockSupabaseClient.Setup(s => s.UploadImage(imageBytes))
                .ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<ProductImageHandleFailedException>(() => _productService.AddAsync(createProductModel));
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
            var category = new Category(Guid.NewGuid(), "Category Name");
            var products = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Product 1", Category = category, CategoryId = category.Id },
                    new Product { Id = Guid.NewGuid(), Name = "Product 2", Category = category, CategoryId = category.Id }
                }.AsQueryable();

            var productsModel = new List<ProductModel>
                {
                    new ProductModel { Id = Guid.NewGuid(), Name = "Product 1", CategoryId = category.Id },
                    new ProductModel { Id = Guid.NewGuid(), Name = "Product 2", CategoryId = category.Id }
                }.AsQueryable();


            _mockRedisService.Setup(r => r.Get<PagedResult<ProductModel>>("test")).Returns((PagedResult<ProductModel>)null);

            _mockRepository.Setup(r => r.GetAllAsync()).Returns(products);
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
            var product = new Product { Id = productId, Name = "Test Product" };
            var productModel = new ProductModel { Id = productId, Name = "Test Product" };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductModel>(product)).Returns(productModel);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(productModel.Name, result.Data.Name);
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
            Assert.NotNull(result.Data);
            Assert.Equal("Update product success", result.Message);
        }
    }
}
