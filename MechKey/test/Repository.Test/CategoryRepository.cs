using Abstraction;
using Domain.Entity;
using Domain.Exceptions;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Repository.Test
{
    public class CategoryRepository
    {
        [Fact]
        public async Task Create_Category_With_Empty_Name_Should_Throw_Exception()
        {
            // Assert
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            using var context = new InMemoryApplicationDbContext(options);
            var repository = new Infrastructure.Repositories.CategoryRepository(context);

            var newCategory = new Category(Guid.NewGuid(), "");

            // Act 
            var exception = await Assert.ThrowsAsync<CategoryValidateFailedException>(() => repository.CreateAsync(newCategory));

            // Assert
            Assert.Equal("Validate Category failed", exception.Message);
        }

        [Fact]
        public async Task Create_Category_Should_Return_New_Category()
        {
            // Assert
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            using var context = new InMemoryApplicationDbContext(options);
            var repository = new Infrastructure.Repositories.CategoryRepository(context);

            var newCategory = new Category(Guid.NewGuid(), "Test 1");

            // Act 
            var entity = await repository.CreateAsync(newCategory);

            // Assert
            Assert.NotNull(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.Equal("Test 1", entity.Name);
            Assert.False(entity.IsDeleted);
        }

        [Fact]
        public async void Edit_Category_Should_Return_Updated_Category()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.Create();
            var repository = new Infrastructure.Repositories.CategoryRepository(context);

            var existingCategory = new Category(Guid.NewGuid(), "Old Category");

            await context.Categories.AddAsync(existingCategory);
            await context.SaveChangesAsync();

            // Act
            existingCategory.Name = "Updated Category"; // thay đổi Name
            await repository.UpdateAsync(existingCategory);

            // Assert
            var updatedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == existingCategory.Id);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Updated Category", updatedCategory.Name);
        }

        [Fact]
        public async void Delete_Category_Should_Return_Entity_With_IsDeleted_True()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.Create();
            var repository = new Infrastructure.Repositories.CategoryRepository(context);

            var existingCategory = new Category(Guid.NewGuid(), "Old Category");

            await context.Categories.AddAsync(existingCategory);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(existingCategory);
            var updatedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == existingCategory.Id);

            // Assert
            Assert.NotNull(updatedCategory);
            Assert.True(updatedCategory.IsDeleted);
        }

        [Fact]
        public async void Get_All_Category_Should_Return_AsQuery()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.Create();
            var repository = new Infrastructure.Repositories.CategoryRepository(context);

            var category1 = new Category
            (
                Guid.NewGuid(),
                "Category 1"
            );
            var category2 = new Category
            (
                Guid.NewGuid(),
                "Category 2"
            );
            var category3 = new Category
            (
                Guid.NewGuid(),
                "Category 3"
            );

            await context.Categories.AddAsync(category1);
            await context.Categories.AddAsync(category2);
            await context.Categories.AddAsync(category3);
            await context.SaveChangesAsync();

            // Act
            var query = repository.GetAllAsync();

            // Assert
            Assert.True(query.Any());
        }


    }
}
