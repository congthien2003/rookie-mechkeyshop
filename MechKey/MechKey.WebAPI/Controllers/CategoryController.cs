using Application.Comoon;
using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Category;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet("list")]
        public async Task<Result<PagedResult<CategoryModel>>> GetList(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default)
        {
            PaginationReqModel paginationReqModel = new PaginationReqModel()
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };
            Result<PagedResult<CategoryModel>> result = await categoryService.GetAllAsync(paginationReqModel, cancellationToken);
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryModel model, CancellationToken cancellationToken = default)
        {
            var entity = await categoryService.AddAsync(model, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = entity.Data.Id }, entity);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(entity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateById(Guid id, CategoryModel model, CancellationToken cancellationToken = default)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");
            var entity = await categoryService.UpdateAsync(model, cancellationToken);
            return Ok(entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await categoryService.DeleteAsync(id, cancellationToken);
            return Ok(result);
        }
    }
}
