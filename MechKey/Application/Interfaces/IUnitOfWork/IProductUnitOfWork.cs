using Domain.Entity;
using Domain.IRepositories;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IProductUnitOfWork : IDisposable
    {
        IProductImageRepository ProductImageRepository { get; }
        IProductRepository<Product> ProductRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
