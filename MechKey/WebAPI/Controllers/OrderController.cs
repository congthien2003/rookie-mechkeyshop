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
            bool asc = true)
        {
            PaginationReqModel pagination = new PaginationReqModel()
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
            };

            var result = await _orderService.GetAllOrders(pagination, startDate, endDate, sortCol, asc);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetOrdersById(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateInfoOrderModel model)
        {
            var result = await _orderService.UpdateOrder(model);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}
