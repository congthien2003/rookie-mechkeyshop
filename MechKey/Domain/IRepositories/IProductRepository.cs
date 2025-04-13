using Domain.Common;

namespace Domain.IRepositories
{
    public interface IProductRepository<Product> : BaseRepository<Product> where Product : class
    {

    }
}
