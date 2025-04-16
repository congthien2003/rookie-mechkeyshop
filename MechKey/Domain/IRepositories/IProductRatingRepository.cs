using Domain.Common;

namespace Domain.IRepositories
{
    public interface IProductRatingRepository<ProductRating> : BaseRepository<ProductRating> where ProductRating : class
    {
        public IQueryable<ProductRating> GetListByProdut(Guid id);
    }
}
