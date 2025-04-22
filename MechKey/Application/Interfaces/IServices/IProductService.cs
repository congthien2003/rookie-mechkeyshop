using Application.Comoon;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface IProductService
    {
        Task<Result<ProductModel>> GetByIdAsync(Guid id);
        Task<Result<ProductModel>> AddAsync(ProductModel model);
        Task<Result<ProductModel>> UpdateAsync(ProductModel user);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<IEnumerable<ProductModel>>> GetBestSellerAsync();
        Task<Result<PagedResult<ProductModel>>> GetAllAsync(PaginationReqModel pagiModel, string categoryId = "", string sortCol = "", bool ascOrder = true);
    }
}
