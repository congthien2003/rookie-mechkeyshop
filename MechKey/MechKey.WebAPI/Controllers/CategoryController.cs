using Application.Catalog.Categories.Commands;
using Application.Catalog.Categories.Queries;
using Application.Comoon;
using Asp.Versioning;
using MediatR;
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
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("list")]
        public async Task<Result<PagedResult<CategoryModel>>> GetList(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default)
        {
            return await _sender.Send(new GetCategoryListQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            }, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryModel model, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateCategoryCommand { Name = model.Name }, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateById(Guid id, CategoryModel model, CancellationToken cancellationToken = default)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");
            var result = await _sender.Send(new UpdateCategoryCommand { Id = model.Id, Name = model.Name }, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new DeleteCategoryCommand { Id = id }, cancellationToken);
            return Ok(result);
        }
    }
}
