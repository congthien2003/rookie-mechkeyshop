using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Common;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ISupabaseService _supabaseService;
        private readonly IEventBus _eventBus;
        private readonly IRedisService _redisService;

        public ProductService(
            IProductRepository<Product> repository,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductUnitOfWork unitOfWork,
            ISupabaseService supabaseService,
            IEventBus eventBus,
            IRedisService redisService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _supabaseService = supabaseService;
            _eventBus = eventBus;
            _redisService = redisService;
        }

        public async Task<Result<ProductModel>> AddAsync(CreateProductModel model)
        {
            // Upload image on cloud
            var uploadImageResponse = await UploadFileAsync(model.ImageData);
            if (!uploadImageResponse.IsSuccess)
                throw new ProductImageHandleFailedException();
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // save to db
                var entity = _mapper.Map<Product>(model);

                ProductImage productImage = new ProductImage
                {
                    FilePath = uploadImageResponse.Data.FilePath,
                    Url = uploadImageResponse.Data.PublicUrl,
                    ProductId = entity.Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.ProductImageRepository.CreateAsync(productImage);

                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                var newEntity = await _unitOfWork.ProductRepository.CreateAsync(entity);

                await _unitOfWork.CommitAsync();

                return Result<ProductModel>.Success("Add product success", _mapper.Map<ProductModel>(newEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Model: {Model}, Message: {Message}", nameof(AddAsync), model, ex.Message);

                // Send event to delete image on cloud
                await _eventBus.PublishAsync(new DeleteImageEvent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Url = uploadImageResponse.Data.PublicUrl
                });

                await _unitOfWork.RollbackAsync();

                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(DeleteAsync), id);
                    throw new ProductNotFoundException();
                }

                await _repository.DeleteAsync(entity);

                // Send event to remove image on cloud
                await _eventBus.PublishAsync(new DeleteImageEvent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Url = entity.ImageUrl
                });
                return Result.Success("Delete product success");
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. ProductId: {ProductId}, Message: {Message}", nameof(DeleteAsync), id, ex.Message);
                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result<PagedResult<ProductModel>>> GetAllAsync(
            PaginationReqModel pagiModel,
            string categoryId = "",
            string sortCol = "",
            bool ascOrder = false)
        {
            string key = $"product-{pagiModel.Page}-{pagiModel.PageSize}-{categoryId}-{pagiModel.SearchTerm}-{sortCol}-{ascOrder}";
            var data = _redisService.Get<PagedResult<ProductModel>>(key);
            if (data is not null)
            {
                return Result<PagedResult<ProductModel>>.Success("Get list product from cache", data);
            }

            var query = _repository.GetAllAsync();

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(q => q.Category.Id.ToString() == categoryId);
            }

            if (!string.IsNullOrEmpty(pagiModel.SearchTerm))
            {
                query = query.Where(u => u.Name.Contains(pagiModel.SearchTerm));
            }

            if (!string.IsNullOrEmpty(sortCol))
            {
                switch (sortCol)
                {
                    case "price":
                        query = ascOrder ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                        break;
                    case "createdAt":
                        query = ascOrder ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                        break;
                    default:
                        break;
                }
            }

            var totalCount = query.Count();
            var items = query
                .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                .Take(pagiModel.PageSize)
                /*.Include(p => p.Category)*/
                /*.Include(p => p.ProductRatings)*/
                .ProjectTo<ProductModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .AsEnumerable();

            data = new PagedResult<ProductModel>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pagiModel.Page,
                PageSize = pagiModel.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize),
            };

            _redisService.Set(key, data, 15);

            return Result<PagedResult<ProductModel>>.Success("Get list product success", data);
        }

        public async Task<Result<IEnumerable<ProductModel>>> GetBestSellerAsync()
        {
            try
            {
                var query = _repository.GetAllAsync();
                var items = await query
                    .OrderByDescending(p => p.SellCount)
                    .Take(4)
                    .Include(p => p.Category)
                    .ProjectTo<ProductModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Result<IEnumerable<ProductModel>>.Success("Get Best seller", items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(GetBestSellerAsync), ex.Message);
                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result<ProductModel>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(GetByIdAsync), id);
                throw new ProductNotFoundException();
            }

            return Result<ProductModel>.Success("Get product by id success", _mapper.Map<ProductModel>(entity));
        }

        public async Task<Result<ProductModel>> UpdateAsync(UpdateProductModel model)
        {
            Result<UploadFileResponseModel> uploadImageResponse = null;
            var entity = await _repository.GetByIdAsync(model.Id);
            if (entity == null)
            {
                _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(UpdateAsync), model.Id);
                throw new ProductNotFoundException();
            }

            // check image update
            if (!string.IsNullOrEmpty(model.ImageData))
            {
                // Upload image on cloud
                uploadImageResponse = await UploadFileAsync(model.ImageData);

                if (uploadImageResponse.IsSuccess)
                {
                    entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                    await _eventBus.PublishAsync(new DeleteImageEvent
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        Url = entity.ImageUrl,
                    });
                }
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.LastUpdatedAt = DateTime.UtcNow;
            entity.Variants = JsonConvert.SerializeObject(model.Variants);
            entity.IsDeleted = model.IsDeleted;

            var result = await _repository.UpdateAsync(entity);
            return Result<ProductModel>.Success("Update product success", _mapper.Map<ProductModel>(result));

        }

        public async Task<Result<UploadFileResponseModel>> UploadFileAsync(string base64string)
        {
            try
            {
                var img = base64string.Substring(23);
                byte[] imageBytes = Convert.FromBase64String(img);

                return await _supabaseService.UploadImage(imageBytes);
            }
            catch (Exception ex)
            {
                throw new ProductImageHandleFailedException();
            }

        }
    }

}
