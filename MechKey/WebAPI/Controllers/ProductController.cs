using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Product;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get(
            int page = 1,
            int pageSize = 10,
            string searchTerm = "",
            string categoryId = "",
            string sortCol = "",
            bool asc = true)
        {
            PaginationReqModel pagination = new PaginationReqModel()
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
            };

            var result = await productService.GetAllAsync(pagination, categoryId, sortCol, asc);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await productService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductModel model)
        {
            var result = await productService.AddAsync(model);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductModel model)
        {
            var result = await productService.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await productService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
