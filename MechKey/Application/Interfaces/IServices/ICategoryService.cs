using Application.Comoon;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<Result<CategoryModel>> GetByIdAsync(Guid id);
        Task<Result<CategoryModel>> AddAsync(CategoryModel model);
        Task<Result<CategoryModel>> UpdateAsync(CategoryModel user);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<PagedResult<CategoryModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "");
    }
}
