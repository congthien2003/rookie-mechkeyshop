using Application.Comoon;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface IProductRatingService
    {
        Task<Result> AddAsync(ProductRatingViewModel model);

        Task<Result<PagedResult<ProductRatingViewModel>>>? GetAllByIdProductAsync(Guid id, int totalItem = 4, bool ascOrder = true);
    }
}
