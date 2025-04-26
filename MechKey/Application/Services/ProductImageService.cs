using Application.Comoon;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.ImageUpload;

namespace Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;
        private readonly IMapper mapper;
        private readonly ILogger<ProductImageService> _logger;
        private readonly ISupabaseService _supabaseService;

        public ProductImageService(IProductImageRepository repository,
            ILogger<ProductImageService> logger,
            IMapper mapper,
            ISupabaseService supabaseService)
        {
            _repository = repository;
            _logger = logger;
            this.mapper = mapper;
            _supabaseService = supabaseService;
        }

        public async Task<Result<string>> AddAsync(UploadFileModel model)
        {
            try
            {
                var img = model.Base64String.Split("data:image/png;base64,");
                byte[] imageBytes = Convert.FromBase64String(img[1]);

                var response = await _supabaseService.UploadImage(imageBytes);

                ProductImage entity = new ProductImage()
                {
                    Id = Guid.NewGuid(),
                    FilePath = response.Data.FileName,
                    Url = response.Data.PublicUrl,
                    ProductId = Guid.Empty,
                };

                var result = await _repository.CreateAsync(entity);

                return Result<string>.Success("Create product image success", result.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Upload file failed");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return Result.Success("Create product image success");
        }

        public async Task<Result<PagedResult<ProductImageModel>>> GetAllAsync(PaginationReqModel pagiModel)
        {
            try
            {
                var query = _repository.GetAllAsync();

                var totalCount = query.Count();

                var items = await query
                    .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                .Take(pagiModel.PageSize)
                    .ProjectTo<ProductImageModel>(mapper.ConfigurationProvider)
                .ToListAsync();

                return Result<PagedResult<ProductImageModel>>.Success("Get images success", new PagedResult<ProductImageModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = pagiModel.Page,
                    PageSize = pagiModel.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get image list");
                return Result<PagedResult<ProductImageModel>>.Failure("Failed to get image list", null);
            }
        }

        public Task<Result<ProductImageModel>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
