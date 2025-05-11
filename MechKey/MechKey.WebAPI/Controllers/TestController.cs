using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Smtp;
using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Order;
using Shared.ViewModels.Product;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IEmailService emailService;
        private readonly IRedisService redisService;

        public TestController(IOrderService orderService, IEmailService emailService, IRedisService redisService)
        {
            this.orderService = orderService;
            this.emailService = emailService;
            this.redisService = redisService;
        }

        [MapToApiVersion(1)]
        [HttpPost]
        public IActionResult CreateProduct(ProductModel model)
        {
            return Ok(model);
        }

        [HttpPost("createOrder")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> CreateOrder(CreateOrderModel model, CancellationToken cancellationToken = default)
        {
            var user = HttpContext.User;
            var id = user.FindFirst("Id")?.Value;

            model.UserId = Guid.Parse(id);

            await orderService.CreateOrder(model, cancellationToken);
            return Ok(model);
        }

        [HttpPost("updateOrder")]
        public async Task<IActionResult> UpdateOrder(UpdateInfoOrderModel model, CancellationToken cancellationToken = default)
        {
            await orderService.UpdateOrder(model, cancellationToken);
            return Ok(model);
        }

        [MapToApiVersion(1)]
        [HttpGet("getOrder")]
        public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await orderService.GetOrdersById(id, cancellationToken);
            return Ok(order);
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail(CancellationToken cancellationToken = default)
        {
            var result = emailService.SendEmailConfirm("buicongthien.01@gmail.com", "123456");
            return Ok(result);
        }

        [HttpPost("order-created")]
        public async Task<IActionResult> SendEmail(OrderModel order, CancellationToken cancellationToken = default)
        {
            var result = emailService.SendEmailOrder("buicongthien.01@gmail.com", order);
            return Ok(result);
        }

        [HttpPost("clear-cache")]
        public async Task<IActionResult> Clear(string key, CancellationToken token = default)
        {
            await redisService.RemoveByPrefixAsync(key, token);
            return Ok();
        }
    }
}
