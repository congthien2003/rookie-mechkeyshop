using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Order;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(
            int page = 1,
            int pageSize = 10,
            string searchTerm = "",
            string startDate = "",
            string endDate = "",
            string sortCol = "",
            bool asc = true,
            CancellationToken cancellationToken = default)
        {
            PaginationReqModel pagination = new PaginationReqModel()
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
            };

            var result = await _orderService.GetAllOrders(pagination, startDate, endDate, sortCol, asc, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _orderService.GetOrdersById(id, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateInfoOrderModel model, CancellationToken cancellationToken = default)
        {
            var result = await _orderService.UpdateOrder(model, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _orderService.DeleteOrder(id, cancellationToken);
            return Ok(result);
        }
    }
}
