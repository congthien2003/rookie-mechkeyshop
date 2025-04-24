using Application.Comoon;
using Shared.ViewModels.ImageUpload;

namespace Application.Interfaces.IServices
{
    public interface ISupabaseService
    {
        Task<Result<UploadFileResponseModel>> UploadImage(byte[] imageBytes);
    }
}
