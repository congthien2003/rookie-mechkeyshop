using Application.Comoon;
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

        public ProductService(
            IProductRepository<Product> repository,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductUnitOfWork unitOfWork,
            ISupabaseService supabaseService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _supabaseService = supabaseService;
        }

        public async Task<Result<ProductModel>> AddAsync(CreateProductModel model)
        {
            try
            {
                var img = model.Base64String.Split("data:image/png;base64,");
                byte[] imageBytes = Convert.FromBase64String(img[1]);

                var uploadImageResponse = await _supabaseService.UploadImage(imageBytes);

                var entity = _mapper.Map<Product>(model);

                if (uploadImageResponse.IsSuccess)
                {
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
                }
                else
                {
                    throw new ProductImageHandleFailedException();
                }

                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                var newEntity = await _unitOfWork.ProductRepository.CreateAsync(entity);

                await _unitOfWork.SaveChangesAsync();

                return Result<ProductModel>.Success("Add product success", _mapper.Map<ProductModel>(newEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Model: {Model}, Message: {Message}", nameof(AddAsync), model, ex.Message);
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
            try
            {
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
                var items = await query
                    .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                    .Take(pagiModel.PageSize)
                    .Include(p => p.Category)
                    .Include(p => p.ProductRatings)
                    .ProjectTo<ProductModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Result<PagedResult<ProductModel>>.Success("Get list product success", new PagedResult<ProductModel>
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
                _logger.LogError(ex, "Error occurred in {Method}. Pagination: {Pagination}, CategoryId: {CategoryId}, SortCol: {SortCol}, AscOrder: {AscOrder}, Message: {Message}",
                    nameof(GetAllAsync), pagiModel, categoryId, sortCol, ascOrder, ex.Message);
                throw new ProductHandleFailedException();
            }
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
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(GetByIdAsync), id);
                    throw new ProductNotFoundException();
                }

                return Result<ProductModel>.Success("Get product by id success", _mapper.Map<ProductModel>(entity));
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. ProductId: {ProductId}, Message: {Message}", nameof(GetByIdAsync), id, ex.Message);
                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result<ProductModel>> UpdateAsync(UpdateProductModel model)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(UpdateAsync), model.Id);
                    throw new ProductNotFoundException();
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;
                entity.ImageUrl = model.ImageUrl;
                entity.LastUpdatedAt = DateTime.UtcNow;
                entity.Variants = JsonConvert.SerializeObject(model.Variants);

                var result = await _repository.UpdateAsync(entity);
                return Result<ProductModel>.Success("Update product success", _mapper.Map<ProductModel>(result));
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. ProductId: {ProductId}, Message: {Message}", nameof(UpdateAsync), model.Id, ex.Message);
                throw new ProductHandleFailedException();
            }
        }
    }
}
