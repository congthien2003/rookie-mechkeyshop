using Application.Catalog.Products.Commands;
using Asp.Versioning;
using MechKey.Application.Catalog.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Product;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get(
            int page = 1,
            int pageSize = 10,
            string searchTerm = "",
            string categoryId = "",
            string sortCol = "",
            bool asc = true,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetProductListQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                SortCol = sortCol,
                Asc = asc
            }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("best-seller")]
        public async Task<IActionResult> GetBestSeller(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetBestSellerProductsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}/ratings")]
        public async Task<IActionResult> GetRatings(Guid id, int pageSize = 4, bool ascOrder = false, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetProductRatingsQuery
            {
                ProductId = id,
                PageSize = pageSize,
                AscOrder = ascOrder
            }, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductModel model, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateProductCommand
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                Variants = model.Variants ?? new(),
                ImageData = model.ImageData
            }, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductModel model, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateProductCommand
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                ImageData = model.ImageData,
                CategoryId = model.CategoryId,
                Variants = model.Variants ?? new(),
                IsDeleted = model.IsDeleted
            }, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new DeleteProductCommand { Id = id }, cancellationToken);
            return Ok(result);
        }
    }
}
