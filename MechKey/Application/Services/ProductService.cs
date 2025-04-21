using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository<Product> _repository;
        private readonly IMapper mapper;
        private readonly ILogger<ProductService> logger;
        public ProductService(IProductRepository<Product> repository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<ProductModel>> AddAsync(ProductModel model)
        {
            try
            {
                var entity = mapper.Map<Product>(model);
                var newEntity = await _repository.CreateAsync(entity);
                return Result<ProductModel>.Success("Add product success", mapper.Map<ProductModel>(newEntity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
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
                logger.LogError(ex, ex.Message);
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
                            {
                                query = ascOrder ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                                break;
                            }
                        case "createdAt":
                            {
                                query = ascOrder ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
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

                return Result<PagedResult<ProductModel>>.Success("Get List user success", new PagedResult<ProductModel>
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
                logger.LogError(ex, ex.Message);
                throw new ProductNotFoundException();
            }
        }

        public async Task<Result<IEnumerable<ProductModel>>> GetBestSellerAsync()
        {
            try
            {
                var query = _repository.GetAllAsync();
                var items = await query
                    .OrderBy(p => p.CreatedAt)
                    .Take(4)
                    .Include(p => p.Category)
                    .ProjectTo<ProductModel>(mapper.ConfigurationProvider) // AutoMapper
                    .ToListAsync();

                return Result<IEnumerable<ProductModel>>.Success("Get Best seller", items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
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

        public async Task<Result<ProductModel>> UpdateAsync(ProductModel model)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                    throw new ProductNotFoundException();

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;
                entity.ImageUrl = model.ImageUrl;
                entity.LastUpdatedAt = DateTime.UtcNow;

                var result = await _repository.UpdateAsync(entity);
                return Result<ProductModel>.Success("Update product success",
                    mapper.Map<ProductModel>(entity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                throw new ProductHandleFailedException();
            }
        }
    }
}
