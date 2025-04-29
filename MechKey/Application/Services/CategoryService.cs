using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Category;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository<Category> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository<Category> repository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CategoryModel>> AddAsync(CreateCategoryModel model)
        {
            if (string.IsNullOrEmpty(model.Name)) throw new CategoryInvalidDataException();
            try
            {
                var entity = new Category(Guid.NewGuid(), model.Name);
                var newEntity = await _repository.CreateAsync(entity);
                return Result<CategoryModel>.Success("Add category success", _mapper.Map<CategoryModel>(newEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Model: {Model}, Message: {Message}", nameof(AddAsync), model, ex.Message);
                throw new CategoryHandleFailedException();
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(DeleteAsync), id);
                throw new CategoryNotFoundException();
            }

            try
            {
                await _repository.DeleteAsync(entity);
                return Result.Success("Delete category success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. CategoryId: {CategoryId}, Message: {Message}", nameof(DeleteAsync), id, ex.Message);
                throw new CategoryHandleFailedException();
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
                _logger.LogError(ex, "Error occurred in {Method}. Pagination: {Pagination}, Message: {Message}", nameof(GetAllAsync), pagiModel, ex.Message);
                throw new Exception("Get category list failed");
            }
        }

        public async Task<Result<CategoryModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(GetByIdAsync), id);
                    throw new CategoryNotFoundException();
                }

                return Result<CategoryModel>.Success("Get category by id success", _mapper.Map<CategoryModel>(entity));
            }
            catch (CategoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. CategoryId: {CategoryId}, Message: {Message}", nameof(GetByIdAsync), id, ex.Message);
                throw new CategoryHandleFailedException();
            }
        }

        public async Task<Result<CategoryModel>> UpdateAsync(CategoryModel model)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(UpdateAsync), model.Id);
                    throw new CategoryNotFoundException();
                }

                entity.Name = model.Name;
                entity.LastUpdatedAt = DateTime.UtcNow;

                var updatedEntity = await _repository.UpdateAsync(entity);
                return Result<CategoryModel>.Success("Update category success", _mapper.Map<CategoryModel>(updatedEntity));
            }
            catch (CategoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. CategoryId: {CategoryId}, Message: {Message}", nameof(UpdateAsync), model.Id, ex.Message);
                throw new CategoryHandleFailedException();
            }
        }
    }

}
