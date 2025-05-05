using Application.Interfaces.IApiClient.Redis;
using Application.Services;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Common;
using Shared.ViewModels.Category;

namespace Application.Test
{
    public class CategoryServiceTest
    {
        private readonly Mock<ICategoryRepository<Category>> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly Mock<IRedisService> _redisServiceMock;
        private readonly CategoryService _categoryService;
        public CategoryServiceTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryModel>().ReverseMap();
            });
            _categoryRepoMock = new Mock<ICategoryRepository<Category>>();
            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup((x) => x.ConfigurationProvider).Returns(config);
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _redisServiceMock = new Mock<IRedisService>();
            _categoryService = new CategoryService(_categoryRepoMock.Object, _mapperMock.Object, _loggerMock.Object, _redisServiceMock.Object);
        }
        private static Category CreateCategoryEntity() => new Category(Guid.NewGuid(), "Category Name");

        [Fact]
        public async Task Add_Category_Async_With_Valid_Data()
        {
            // Arrange
            const string name = "Name test";
            CreateCategoryModel model = new CreateCategoryModel()
            {
                Name = name,
            };

            // Act
            var result = await _categoryService.AddAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(repo =>
            repo.CreateAsync(It.Is<Category>(c => c.Name == "Name test")), Times.Once);

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

            await Assert.ThrowsAsync<CategoryInvalidDataException>(() => _categoryService.AddAsync(model));

            _categoryRepoMock.Verify(repo =>
            repo.CreateAsync(It.IsAny<Category>()), Times.Never);

        }

        [Fact]
        public async Task Add_Category_Async_Should_Throw_HandleFailedException()
        {
            // Arrange
            const string name = "";
            CreateCategoryModel model = new CreateCategoryModel()
            {
                Name = name,
            };
            Category category = new Category(Guid.NewGuid(), "Test");
            _categoryRepoMock.Setup(repo => repo.CreateAsync(category)).Throws(new CategoryInvalidDataException());
            // Assert
            await Assert.ThrowsAsync<CategoryHandleFailedException>(() => _categoryService.AddAsync(model));

            _categoryRepoMock.Verify(repo =>
            repo.CreateAsync(It.IsAny<Category>()), Times.Once);

        }


        [Fact]
        public async Task Update_Category_Async_With_Valid_Data_Should_Update_Category()
        {
            // Arrange
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(category);
            _categoryRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>()));

            // Act
            category.Name = "Category Updated";
            CategoryModel categoryModel = new CategoryModel
            {
                Name = category.Name,
                Id = category.Id
            };
            var result = await _categoryService.UpdateAsync(categoryModel);

            // Assert
            Assert.True(result.IsSuccess);

            _categoryRepoMock.Verify(repo =>
                    repo.UpdateAsync(It.Is<Category>(c => c.Name == "Category Updated")), Times.Once);
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
                r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.UpdateAsync(categoryModel));
        }

        [Fact]
        public async Task Delete_Category_Async_With_Valid_Id_Should_Delete_Category()
        {
            // Arange
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(category);
            _categoryRepoMock.Setup(repo => repo.DeleteAsync(It.IsAny<Category>()));

            // Act
            var result = await _categoryService.DeleteAsync(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(repo => repo.DeleteAsync(category), Times.Once);
        }

        [Fact]
        public async Task Delete_Category_Async_With_Invalid_Id_Should_Throw()
        {
            // Arange
            var category = CreateCategoryEntity();
            var Invalid_ID = Guid.NewGuid();
            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Category)null);
            _categoryRepoMock.Setup(repo => repo.DeleteAsync(It.IsAny<Category>()));

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.DeleteAsync(Invalid_ID));

            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _categoryRepoMock.Verify(repo =>
                repo.DeleteAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task Get_Category_By_Id_Async_Should_Return_Category_If_Exists()
        {
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var result = await _categoryService.GetByIdAsync(category.Id);

            Assert.NotNull(result);
            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Get_Category_By_Id_Async_Should_Throw_If_Not_Exists()
        {
            var category = CreateCategoryEntity();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid()))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.GetByIdAsync(category.Id));

            _categoryRepoMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Get_List_Category_With_Search_Paging()
        {
            // Arrange
            const int page = 1;
            const int pageSize = 10;

            var allData = Enumerable.Range(1, 20)
                .Select(i => new Category(Guid.NewGuid(), $"Category {i}"))
                .AsQueryable();

            // Giả lập repository trả ra full list
            _redisServiceMock.Setup(r => r.Get<PagedResult<CategoryModel>>(It.IsAny<string>())).Returns((PagedResult<CategoryModel>)null);
            _categoryRepoMock.Setup(r => r.GetAllAsync())
                .Returns(allData); // đổi từ IQueryable -> List

            var model = new PaginationReqModel
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = "" // chưa search
            };

            // Act
            var result = await _categoryService.GetAllAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.Data.Page);
            Assert.Equal(pageSize, result.Data.PageSize);
            Assert.Equal(pageSize, result.Data.Items.Count()); // đúng số lượng phân trang

            _categoryRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

    }
}
