using Application.Comoon;
using Shared.ViewModels.ImageUpload;

namespace Application.Interfaces.IApiClient.Supabase
{
    public interface ISupabaseService
    {
        Task<Result<UploadFileResponseModel>> UploadImage(byte[] imageBytes);
    }
}
