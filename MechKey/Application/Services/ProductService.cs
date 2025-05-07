using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using Application.Validators;
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
        private readonly string patternCache = "product";
        private readonly IProductRatingRepository<ProductRating> _ratingRepository;

        public ProductService(
            IProductRepository<Product> repository,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductUnitOfWork unitOfWork,
            ISupabaseService supabaseService,
            IEventBus eventBus,
            IRedisService redisService,
            IProductRatingRepository<ProductRating> ratingRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _supabaseService = supabaseService;
            _eventBus = eventBus;
            _redisService = redisService;
            _ratingRepository = ratingRepository;
        }

        public async Task<Result<ProductModel>> AddAsync(CreateProductModel model, CancellationToken cancellationToken = default)
        {
            ProductValidator.CreatedProductValidator(model);

            // Upload image on cloud
            var uploadImageResponse = await UploadFileAsync(model.ImageData, cancellationToken);
            if (!uploadImageResponse.IsSuccess)
                throw new ProductImageHandleFailedException();
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

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

                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                var newEntity = await _unitOfWork.ProductRepository.CreateAsync(entity, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
                await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);
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
                }, cancellationToken);

                await _unitOfWork.RollbackAsync(cancellationToken);

                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {

            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(DeleteAsync), id);
                throw new ProductNotFoundException();
            }

            await _repository.DeleteAsync(entity, cancellationToken);
            await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);

            // Send event to remove image on cloud
            /*            await _eventBus.PublishAsync(new DeleteImageEvent
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            Url = entity.ImageUrl
                        }, cancellationToken);
            */
            return Result.Success("Delete product success");
        }

        public async Task<Result<PagedResult<ProductModel>>> GetAllAsync(
            PaginationReqModel pagiModel,
            string categoryId = "",
            string sortCol = "",
            bool ascOrder = false,
            CancellationToken cancellationToken = default)
        {
            string key = $"{patternCache}-{pagiModel.Page}-{pagiModel.PageSize}-{categoryId}-{pagiModel.SearchTerm}-{sortCol}-{ascOrder}";
            var data = _redisService.Get<PagedResult<ProductModel>>(key);
            if (data is not null)
            {
                return Result<PagedResult<ProductModel>>.Success("Get list product from cache", data);
            }

            var query = _repository.GetAllAsync().AsNoTracking();

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

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(i => i.Id)
                .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                .Take(pagiModel.PageSize)
                .Select(p => new ProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name ?? null,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    SellCount = p.SellCount,
                    IsDeleted = p.IsDeleted,
                    TotalRating = p.ProductRatings.Count > 0
                        ? p.ProductRatings.Average(r => r.Stars)
                        : 0,
                    Variants = string.IsNullOrEmpty(p.Variants)
                    ? new List<VariantAttribute>()
                    : JsonConvert.DeserializeObject<List<VariantAttribute>>(p.Variants)
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            data = new PagedResult<ProductModel>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pagiModel.Page,
                PageSize = pagiModel.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize),
            };

            await _redisService.Set(key, data, 15);

            return Result<PagedResult<ProductModel>>.Success("Get list product success", data);
        }

        public async Task<Result<IEnumerable<ProductModel>>> GetBestSellerAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _repository.GetAllAsync();
                var items = await query
                    .OrderByDescending(p => p.SellCount)
                    .Take(4)
                    .Include(p => p.Category)
                    .ProjectTo<ProductModel>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<IEnumerable<ProductModel>>.Success("Get Best seller", items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(GetBestSellerAsync), ex.Message);
                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result<ProductModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(GetByIdAsync), id);
                throw new ProductNotFoundException();
            }
            /*
                        var listRating = _ratingRepository.GetListByProduct(entity.Id);
                        listRating.Select(p => new ProductRatingModel
                        {
                            Id = p.Id,
                            ProductId = p.ProductId,
                            UserId = p.UserId,
                            Name = p.User.Name,
                            RatedAt = p.RatedAt,
                            Comment = p.Comment,
                            Stars = p.Stars
                        }).ToListAsync();*/

            ProductModel result = new ProductModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category.Name ?? null,
                ImageUrl = entity.ImageUrl,
                Price = entity.Price,
                SellCount = entity.SellCount,
                IsDeleted = entity.IsDeleted,
                Rating = entity.ProductRatings.Select(p => new ProductRatingModel
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    UserId = p.UserId,
                    Name = p.User.Name,
                    RatedAt = p.RatedAt,
                    Comment = p.Comment,
                    Stars = p.Stars
                }).ToList(),
                TotalRating = entity.ProductRatings.Count > 0
                        ? entity.ProductRatings.Average(r => r.Stars)
                        : 0,
                Variants = string.IsNullOrEmpty(entity.Variants)
                    ? new List<VariantAttribute>()
                    : JsonConvert.DeserializeObject<List<VariantAttribute>>(entity.Variants)
            };

            return Result<ProductModel>.Success("Get product by id success", result);
        }

        public async Task<Result<ProductModel>> UpdateAsync(UpdateProductModel model, CancellationToken cancellationToken = default)
        {
            ProductValidator.UpdatedProductValidator(model);

            Result<UploadFileResponseModel> uploadImageResponse = null;
            var entity = await _repository.GetByIdAsync(model.Id, cancellationToken);
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
                    }, cancellationToken);
                }
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.LastUpdatedAt = DateTime.UtcNow;
            entity.Variants = JsonConvert.SerializeObject(model.Variants);
            entity.IsDeleted = model.IsDeleted;

            var result = await _repository.UpdateAsync(entity, cancellationToken);
            await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);

            return Result<ProductModel>.Success("Update product success", _mapper.Map<ProductModel>(result));
        }

        public async Task<Result<UploadFileResponseModel>> UploadFileAsync(string base64string, CancellationToken cancellationToken = default)
        {
            var base64Parts = base64string.Split(',');
            if (base64Parts.Length != 2)
            {
                throw new InvalidDataException("Base64 is invalid"); // bạn có thể định nghĩa exception riêng
            }

            byte[] imageBytes = Convert.FromBase64String(base64Parts[1]);

            return await _supabaseService.UploadImage(imageBytes);
        }
    }
}
