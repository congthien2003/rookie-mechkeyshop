using Domain.Common;

namespace Domain.IRepositories
{
    public interface IProductRatingRepository<ProductRating> : BaseRepository<ProductRating> where ProductRating : class
    {

    }
}
