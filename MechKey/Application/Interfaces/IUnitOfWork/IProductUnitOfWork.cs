using Domain.Entity;
using Domain.IRepositories;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IProductUnitOfWork : BaseUnitOfWork
    {
        IProductImageRepository ProductImageRepository { get; }
        IProductRepository<Product> ProductRepository { get; }
    }
}
