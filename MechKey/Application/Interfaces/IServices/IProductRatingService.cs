using Application.Comoon;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface IProductRatingService
    {
        Task<Result> AddAsync(ProductRatingModel model);

        Task<Result<PagedResult<ProductRatingModel>>>? GetAllByIdProductAsync(Guid id, int totalItem = 4, bool ascOrder = true);
    }
}
