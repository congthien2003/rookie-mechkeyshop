using Domain.Entity;
using Domain.Exceptions;
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

        public async Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entity.Name))
                throw new CategoryValidateFailedException();
            var result = await context.Categories.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            context.Categories.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<Category> GetAllAsync()
        {
            return context.Categories
                .Include(c => c.Products)
                .AsQueryable();
        }

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Category> UpdateAsync(Category entity, CancellationToken cancellationToken = default)
        {
            context.Categories.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
