using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
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
        private readonly IRedisService _redisService;
        private readonly string patternCache = "category";

        public CategoryService(
            ICategoryRepository<Category> repository,
            IMapper mapper,
            ILogger<CategoryService> logger,
            IRedisService redisService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _redisService = redisService;
        }

        public async Task<Result<CategoryModel>> AddAsync(CreateCategoryModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(model.Name)) throw new CategoryInvalidDataException();

            var entity = new Category(Guid.NewGuid(), model.Name);
            var newEntity = await _repository.CreateAsync(entity, cancellationToken);
            await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);

            return Result<CategoryModel>.Success("Add category success", _mapper.Map<CategoryModel>(newEntity));
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(DeleteAsync), id);
                throw new CategoryNotFoundException();
            }

            await _repository.DeleteAsync(entity, cancellationToken);
            await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);

            return Result.Success("Delete category success");
        }

        public async Task<Result<PagedResult<CategoryModel>>> GetAllAsync(PaginationReqModel pagiModel, CancellationToken cancellationToken = default)
        {
            string key = $"{patternCache}-{pagiModel.Page}-{pagiModel.PageSize}";
            var data = _redisService.Get<PagedResult<CategoryModel>>(key);

            if (data is null)
            {
                var query = _repository.GetAllAsync();

                if (!string.IsNullOrEmpty(pagiModel.SearchTerm))
                {
                    query = query.Where(c => c.Name.Contains(pagiModel.SearchTerm.ToString()));
                }

                int totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((pagiModel.Page - 1) * pagiModel.PageSize)
                    .Take(pagiModel.PageSize)
                    .Select(c => _mapper.Map<CategoryModel>(c))
                    .ToListAsync(cancellationToken);

                data = new PagedResult<CategoryModel>
                {
                    Items = items,
                    TotalItems = totalCount,
                    Page = pagiModel.Page,
                    PageSize = pagiModel.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize)
                };

                await _redisService.Set(key, data, 120);
            }

            return Result<PagedResult<CategoryModel>>.Success("Get category list success", data);
        }

        public async Task<Result<CategoryModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(GetByIdAsync), id);
                throw new CategoryNotFoundException();
            }

            return Result<CategoryModel>.Success("Get category by id success", _mapper.Map<CategoryModel>(entity));
        }

        public async Task<Result<CategoryModel>> UpdateAsync(CategoryModel model, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(model.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(UpdateAsync), model.Id);
                throw new CategoryNotFoundException();
            }

            entity.Name = model.Name;
            entity.LastUpdatedAt = DateTime.UtcNow;

            var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);
            await _redisService.RemoveByPrefixAsync(patternCache, cancellationToken);

            return Result<CategoryModel>.Success("Update category success", _mapper.Map<CategoryModel>(updatedEntity));
        }
    }
}
