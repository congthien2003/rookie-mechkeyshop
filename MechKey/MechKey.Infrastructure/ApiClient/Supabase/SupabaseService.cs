using Application.Comoon;
using Application.Interfaces.IApiClient.Supabase;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.ViewModels.ImageUpload;
using System.Net.Http.Headers;

namespace Infrastructure.ApiClient.SupabaseCloud
{
    public class SupabaseService : ISupabaseService
    {
        private readonly string SUPABASE_URL;
        private readonly string SUPABASE_KEY;
        private readonly ILogger<SupabaseService> _logger;
        public SupabaseService(IConfiguration config, ILogger<SupabaseService> logger)
        {
            SUPABASE_URL = config.GetSection("Supabase:URL").Value!;
            SUPABASE_KEY = config.GetSection("Supabase:KEY").Value!;
            Console.WriteLine(SUPABASE_URL);
            _logger = logger;
        }
        public async Task<Result<UploadFileResponseModel>> UploadImage(byte[] imageBytes)
        {
            var supabase = new Supabase.Client(SUPABASE_URL, SUPABASE_KEY);
            await supabase.InitializeAsync();
            using var stream = new MemoryStream(imageBytes);
            var BUCKET = "products";
            var FILE_PATH = Guid.NewGuid().ToString() + "_product";

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{SUPABASE_URL}/storage/v1/object/{BUCKET}/{FILE_PATH}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SUPABASE_KEY);
            request.Headers.Add("x-upsert", "true");

            var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Headers.Add("Content-Disposition", "inline"); // 👈 quan trọng

            request.Content = content;

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var uploadFileResponseModel = new UploadFileResponseModel
                {
                    FileName = FILE_PATH,
                    FilePath = FILE_PATH,
                    PublicUrl = $"{SUPABASE_URL}/storage/v1/object/public/{BUCKET}/{FILE_PATH}",
                };

                return Result<UploadFileResponseModel>.Success("Get success", uploadFileResponseModel);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Upload failed: " + error);

                throw new ProductImageHandleFailedException();
            }
        }

        public async Task<bool> DeleteImage(string publicURL)
        {
            using var httpClient = new HttpClient();
            var url = publicURL.Split("/public/");


            var request = new HttpRequestMessage(HttpMethod.Delete,
                $"{url[0]}/{url[1]}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SUPABASE_KEY);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Delete failed: " + error);
                throw new Exception(error);
            }
        }
    }
}
