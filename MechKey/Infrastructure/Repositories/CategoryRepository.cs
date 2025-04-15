using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository<Category>
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Category> CreateAsync(Category entity)
        {
            var result = await context.Categories.AddAsync(entity);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Category entity)
        {
            entity.IsDeleted = true;
            context.Categories.Update(entity);
            await context.SaveChangesAsync();
        }

        public IQueryable<Category> GetAllAsync()
        {
            return context.Categories.AsQueryable();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> UpdateAsync(Category entity)
        {
            context.Categories.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
