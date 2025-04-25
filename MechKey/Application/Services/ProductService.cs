using Application.Comoon;
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
        private readonly IMapper mapper;
        private readonly ILogger<ProductService> logger;
        private readonly IProductUnitOfWork unitOfWork;
        private readonly ISupabaseService supabaseService;
        public ProductService(IProductRepository<Product> repository,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductUnitOfWork unitOfWork,
            ISupabaseService supabaseService)
        {
            _repository = repository;
            this.mapper = mapper;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.supabaseService = supabaseService;
        }

        public async Task<Result<ProductModel>> AddAsync(CreateProductModel model)
        {
            try
            {
                var img = model.Base64String.Split("data:image/png;base64,");
                byte[] imageBytes = Convert.FromBase64String(img[1]);

                var uploadImageResponse = await supabaseService.UploadImage(imageBytes);

                var entity = mapper.Map<Product>(model);

                if (uploadImageResponse.IsSuccess)
                {

                    ProductImage productImage = new ProductImage()
                    {
                        FilePath = uploadImageResponse.Data.FilePath,
                        Url = uploadImageResponse.Data.PublicUrl,
                        ProductId = entity.Id,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                    };

                    await unitOfWork.ProductImageRepository.CreateAsync(productImage);
                }
                else
                {
                    throw new ProductImageHandleFailedException();
                }

                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                var newEntity = await unitOfWork.ProductRepository.CreateAsync(entity);

                await unitOfWork.SaveChangesAsync();

                return Result<ProductModel>.Success("Add product success", mapper.Map<ProductModel>(newEntity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message.ToString());
                throw new ProductHandleFailedException();
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ProductNotFoundException();
            }

            try
            {
                await _repository.DeleteAsync(entity);
                return Result.Success("Delete product success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message.ToString());
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

                if (!string.IsNullOrEmpty(pagiModel.SearchTerm.ToString()))
                {
                    query = query.Where(u => u.Name.Contains(pagiModel.SearchTerm.ToString()));
                }

                if (!string.IsNullOrEmpty(sortCol))
                {
                    switch (sortCol)
                    {
                        case "price":
                            {
                                query = ascOrder ? query.OrderBy(p => p.Price.ToString()) : query.OrderByDescending(p => p.Price.ToString());
                                break;
                            }
                        case "createdAt":
                            {
                                query = ascOrder ? query.OrderBy(p => p.CreatedAt.ToString()) : query.OrderByDescending(p => p.CreatedAt.ToString());
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                }

                var totalCount = query.Count();
                var items = await query
                    .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                    .Take(pagiModel.PageSize)
                    .Include(p => p.Category)
                    .Include(p => p.ProductRatings)
                    .ProjectTo<ProductModel>(mapper.ConfigurationProvider) // AutoMapper
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
                logger.LogError(ex, ex.Message.ToString());
                throw new ProductNotFoundException();
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
                    .ProjectTo<ProductModel>(mapper.ConfigurationProvider) // AutoMapper
                    .ToListAsync();

                return Result<IEnumerable<ProductModel>>.Success("Get Best seller", items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message.ToString());
                throw new ProductNotFoundException();
            }
        }

        public async Task<Result<ProductModel>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ProductNotFoundException();
            }

            return Result<ProductModel>.Success("Get product by id success",
                mapper.Map<ProductModel>(entity));
        }


        public async Task<Result<ProductModel>> UpdateAsync(UpdateProductModel model)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                    throw new ProductNotFoundException();

                entity.Name = model.Name.ToString();
                entity.Description = model.Description.ToString();
                entity.Price = model.Price;
                entity.ImageUrl = model.ImageUrl.ToString();
                entity.LastUpdatedAt = DateTime.UtcNow;
                entity.Variants = JsonConvert.SerializeObject(model.Variants);

                var result = await _repository.UpdateAsync(entity);
                return Result<ProductModel>.Success("Update product success",
                    mapper.Map<ProductModel>(entity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message.ToString());

                throw new ProductHandleFailedException();
            }
        }
    }
}
