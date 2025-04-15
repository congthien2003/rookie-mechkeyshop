using Application.Comoon;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface IProductRatingService
    {
        Task<Result> AddAsync(ProductRatingViewModel model);
    }
}
