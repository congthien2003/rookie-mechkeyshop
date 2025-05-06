using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Product;

namespace Application.Interfaces.IServices
{
    public interface IProductService
    {
        Task<Result<ProductModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ProductModel>> AddAsync(CreateProductModel model, CancellationToken cancellationToken = default);
        Task<Result<ProductModel>> UpdateAsync(UpdateProductModel user, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ProductModel>>> GetBestSellerAsync(CancellationToken cancellationToken = default);
        Task<Result<PagedResult<ProductModel>>> GetAllAsync(PaginationReqModel pagiModel, string categoryId = "", string sortCol = "", bool ascOrder = true, CancellationToken cancellationToken = default);
    }
}
