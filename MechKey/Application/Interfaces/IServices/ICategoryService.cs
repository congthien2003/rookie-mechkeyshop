using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Category;

namespace Application.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<Result<CategoryModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<CategoryModel>> AddAsync(CreateCategoryModel model, CancellationToken cancellationToken = default);
        Task<Result<CategoryModel>> UpdateAsync(CategoryModel user, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<CategoryModel>>> GetAllAsync(PaginationReqModel pagiModel, CancellationToken cancellationToken = default);
    }
}
