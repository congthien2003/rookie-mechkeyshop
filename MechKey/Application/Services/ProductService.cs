using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository<Product> _repository;
        private readonly IMapper mapper;
        public ProductService(IProductRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            this.mapper = mapper;
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
                throw new Exception("Add Product failed");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception("Not found product to delete");
            }

            try
            {
                await _repository.DeleteAsync(entity);
                return Result.Success("Delete product success");
            }
            catch (Exception ex)
            {
                throw new Exception("Delete product failed");
            }
        }

        public async Task<Result<PagedResult<ProductModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var query = await _repository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.Name.Contains(searchTerm));
                }

                var totalCount = query.Count();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<ProductModel>(mapper.ConfigurationProvider) // AutoMapper
                    .ToListAsync();

                return Result<PagedResult<ProductModel>>.Success("Get List user success", new PagedResult<ProductModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Get list failed");
            }
        }

        public async Task<Result<ProductModel>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception("Not found product ");
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
                    throw new KeyNotFoundException("Not found product by id");

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
                throw new Exception("Update product failed");
            }
        }
    }
}
