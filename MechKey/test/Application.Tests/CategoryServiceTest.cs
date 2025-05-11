using Application.Interfaces.IApiClient.Redis;
using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Shared.Common;
using Shared.Mapping;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Test
{
    public class CategoryServiceTest
    {
        private readonly Mock<ICategoryRepository<Category>> _categoryRepoMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly Mock<IRedisService> _redisServiceMock;
        private readonly Mock<ICategoryMapping> _categoryMappingMock;
        private readonly CategoryService _categoryService;
        private readonly CancellationToken _cancellationToken;

        public CategoryServiceTest()
        {
            _categoryRepoMock = new Mock<ICategoryRepository<Category>>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _redisServiceMock = new Mock<IRedisService>();
            _categoryMappingMock = new Mock<ICategoryMapping>();
            _categoryService = new CategoryService(_categoryRepoMock.Object, _loggerMock.Object, _redisServiceMock.Object, _categoryMappingMock.Object);
            _cancellationToken = new CancellationToken();
        }

        private static Category CreateCategoryEntity() => new Category() { Id = Guid.NewGuid(), Name = "Test category" };

        [Fact]
        public async Task Add_Category_Async_With_Valid_Data()
        {
            // Arrange
            const string name = "Name test";
            CreateCategoryModel model = new CreateCategoryModel()
            {
                Name = name,
            };
            var category = CreateCategoryEntity();

            _categoryMappingMock.Setup(r => r.ToCategoryByCreatedCategoryModel(model)).Returns(category);
            _categoryRepoMock.Setup(r => r.CreateAsync(category, _cancellationToken)).ReturnsAsync(category);
            // Act
            var result = await _categoryService.AddAsync(model, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(repo =>
            repo.CreateAsync(It.Is<Category>(c => c.Name == "Name test"), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Add_Category_Async_With_Empty_Name_Should_Throw_InvalidDataException()
        {
            // Arrange
            const string name = "";
            CreateCategoryModel model = new CreateCategoryModel()
            {
                Name = name,
            };

            // Assert
            await Assert.ThrowsAsync<CategoryInvalidDataException>(() => _categoryService.AddAsync(model, _cancellationToken));

            _categoryRepoMock.Verify(repo =>
            repo.CreateAsync(It.IsAny<Category>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Update_Category_Async_With_Valid_Data_Should_Update_Category()
        {
            // Arrange
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(category);
            _categoryRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>(), _cancellationToken));

            // Act
            category.Name = "Category Updated";
            CategoryModel categoryModel = new CategoryModel
            {
                Name = category.Name,
                Id = category.Id
            };
            var result = await _categoryService.UpdateAsync(categoryModel, _cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);

            _categoryRepoMock.Verify(repo =>
                    repo.UpdateAsync(It.Is<Category>(c => c.Name == "Category Updated"), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Update_Category_Async_With_Not_Exists_Category_Should_Throw()
        {
            CategoryModel categoryModel = new CategoryModel
            {
                Name = "Test",
                Id = Guid.NewGuid()
            };
            _categoryRepoMock.Setup(r =>
                r.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.UpdateAsync(categoryModel, _cancellationToken));
        }

        [Fact]
        public async Task Delete_Category_Async_With_Valid_Id_Should_Delete_Category()
        {
            // Arrange
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(category);
            _categoryRepoMock.Setup(repo => repo.DeleteAsync(It.IsAny<Category>(), _cancellationToken));

            // Act
            var result = await _categoryService.DeleteAsync(category.Id, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(repo => repo.DeleteAsync(category, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Delete_Category_Async_With_Invalid_Id_Should_Throw()
        {
            // Arrange
            var category = CreateCategoryEntity();
            var invalidId = Guid.NewGuid();
            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
                .ReturnsAsync((Category)null);
            _categoryRepoMock.Setup(repo => repo.DeleteAsync(It.IsAny<Category>(), _cancellationToken));

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.DeleteAsync(invalidId, _cancellationToken));

            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken), Times.Once);
            _categoryRepoMock.Verify(repo =>
                repo.DeleteAsync(It.IsAny<Category>(), _cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Get_Category_By_Id_Async_Should_Return_Category_If_Exists()
        {
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, _cancellationToken))
                .ReturnsAsync(category);

            var result = await _categoryService.GetByIdAsync(category.Id, _cancellationToken);

            Assert.NotNull(result);
            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Get_Category_By_Id_Async_Should_Throw_If_Not_Exists()
        {
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid(), _cancellationToken))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.GetByIdAsync(category.Id, _cancellationToken));

            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Get_List_Category_With_Search_Paging()
        {
            // Arrange
            const int page = 1;
            const int pageSize = 10;

            var allData = Enumerable.Range(1, 20)
                .Select(i => new Category { Id = Guid.NewGuid(), Name = $"Category {i}" })
                .ToList();

            var mockQueryable = allData.AsQueryable().BuildMock();

            _redisServiceMock.Setup(r => r.Get<PagedResult<CategoryModel>>(It.IsAny<string>())).Returns((PagedResult<CategoryModel>)null);
            _categoryRepoMock.Setup(r => r.GetAllAsync())
                .Returns(mockQueryable);

            var model = new PaginationReqModel
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = ""
            };

            // Act
            var result = await _categoryService.GetAllAsync(model, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.Data.Page);
            Assert.Equal(pageSize, result.Data.PageSize);
            Assert.Equal(pageSize, result.Data.Items.Count());

            _categoryRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
