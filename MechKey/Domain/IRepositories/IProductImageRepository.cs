using Domain.Entity;

namespace Domain.IRepositories
{
    public interface IProductImageRepository
    {
        Task<ProductImage> CreateAsync(ProductImage entity);
        Task DeleteAsync(Guid id);
        Task<ProductImage> GetByIdAsync(Guid id);
        IQueryable<ProductImage> GetAllAsync();
    }
}
