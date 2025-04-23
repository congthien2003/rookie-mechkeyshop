using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.ViewModels.Category;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<CategoryModel>> AddAsync(CreateCategoryModel model)
        {
            try
            {
                var entity = new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                };
                var newEntity = await _repository.CreateAsync(entity);
                return Result<CategoryModel>.Success("Add category success", _mapper.Map<CategoryModel>(newEntity));
            }
            catch (Exception ex)
            {
                throw new Exception("Add category failed", ex);
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            try
            {
                await _repository.DeleteAsync(entity);
                return Result.Success("Delete category success");
            }
            catch (Exception ex)
            {
                throw new Exception("Delete category failed", ex);
            }
        }

        public async Task<Result<PagedResult<CategoryModel>>> GetAllAsync(PaginationReqModel pagiModel)
        {
            try
            {
                var query = _repository.GetAllAsync();

                if (!string.IsNullOrEmpty(pagiModel.SearchTerm))
                {
                    query = query.Where(c => c.Name.Contains(pagiModel.SearchTerm.ToString()));
                }

                int totalCount = query.ToList().Count;

                var items = await query
                    .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                    .Take(pagiModel.PageSize)
                    .Select(c => _mapper.Map<CategoryModel>(c))
                    .ToListAsync();
                return Result<PagedResult<CategoryModel>>.Success("Get category list success", new PagedResult<CategoryModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = pagiModel.Page,
                    PageSize = pagiModel.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize)
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Get category list failed", ex);
            }
        }

        public async Task<Result<CategoryModel>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return Result<CategoryModel>.Success("Get category by id success", _mapper.Map<CategoryModel>(entity));
        }

        public async Task<Result<CategoryModel>> UpdateAsync(CategoryModel model)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                    throw new KeyNotFoundException("Category not found");

                entity.Name = model.Name;
                entity.LastUpdatedAt = DateTime.UtcNow;

                var updatedEntity = await _repository.UpdateAsync(entity);
                return Result<CategoryModel>.Success("Update category success", _mapper.Map<CategoryModel>(updatedEntity));
            }
            catch (Exception ex)
            {
                throw new Exception("Update category failed", ex);
            }
        }
    }

}
