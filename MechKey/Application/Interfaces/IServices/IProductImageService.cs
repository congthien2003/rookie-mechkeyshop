using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.ImageUpload;

namespace Application.Interfaces.IServices
{
    public interface IProductImageService
    {
        Task<Result<ProductImageModel>> GetByIdAsync(Guid id);
        Task<Result<string>> AddAsync(UploadFileModel model);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<PagedResult<ProductImageModel>>> GetAllAsync(PaginationReqModel pagiModel);
    }
}
