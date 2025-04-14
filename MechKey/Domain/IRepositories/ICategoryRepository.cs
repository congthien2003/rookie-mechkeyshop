using Domain.Common;

namespace Domain.IRepositories
{
    public interface ICategoryRepository<Category> : BaseRepository<Category> where Category : class
    {
    }
}
