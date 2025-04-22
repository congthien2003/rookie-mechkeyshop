using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Category;

namespace Application.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<Result<CategoryModel>> GetByIdAsync(Guid id);
        Task<Result<CategoryModel>> AddAsync(CreateCategoryModel model);
        Task<Result<CategoryModel>> UpdateAsync(CategoryModel user);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<PagedResult<CategoryModel>>> GetAllAsync(PaginationReqModel pagiModel);
    }
}
