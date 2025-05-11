using Domain.Entity;
using Domain.IRepositories;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IProductUnitOfWork : BaseUnitOfWork
    {
        IProductRepository<Product> ProductRepository { get; }
    }
}
