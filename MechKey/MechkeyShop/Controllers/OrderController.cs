using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Order;

namespace MechkeyShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("/api/Order/createOrder")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderModel model)
        {
            var user = HttpContext.User;
            var id = user.FindFirst("Id")?.Value;

            model.UserId = Guid.Parse(id);
            model.OrderDate = DateTime.UtcNow;

            var result = await orderService.CreateOrder(model);
            return BadRequest("Loi");
            return Ok(result);
        }

        [HttpGet("/Order/Detail/{id:Guid}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var order = await orderService.GetOrdersById(id);

            return View(order);
        }

        [HttpGet]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("Id")?.Value;

            PaginationReqModel pagiModel = new PaginationReqModel()
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = ""
            };

            var result = await orderService.GetAllOrdersByIdUser(Guid.Parse(userId), pagiModel, "createdAt", false);

            return View(result.Data);
        }
    }
}
