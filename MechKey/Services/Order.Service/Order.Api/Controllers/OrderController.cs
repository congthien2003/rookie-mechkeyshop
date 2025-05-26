using EventBus;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Order.Application.Interfaces;
using Order.Domain.Agreegates;
using Order.Domain.Events.Domain;
using Order.Domain.ValueObjects;
using Order.Shared.DTOs.Requests;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEventStore _eventStore;
        private readonly IReadOrderService _readOrderService;
        private readonly IEventBus eventBus;

        public OrderController(IEventStore eventStore, IReadOrderService readOrderService, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _readOrderService = readOrderService;
            this.eventBus = eventBus;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(CreateOrderReq req, CancellationToken token)
        {
            var items = new List<OrderItem>
                {
                    new() { ProductId = "1", ProductName = "Product A", Price = 10000, Quantity = 1 }
                };
            var orderId = Guid.NewGuid();

            var aggregate = OrderAggregate.Create(orderId, "cus-001", items);
            await _eventStore.SaveEventsAsync(orderId, aggregate.GetUncommittedEvents());

            var reloadedEvents = await _eventStore.GetEventsAsync(orderId);

            await eventBus.PublishAsync(
                new OrderCreatedDomainEvent(
                    aggregate.Id.ToString(),
                    aggregate.CustomerId,
            aggregate.Items.Sum(c => c.Quantity * c.Price),
                    DateTime.UtcNow), token);
            return Ok(reloadedEvents);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById()
        {
            var orderId = "ce0f1b1e-9694-4834-b998-46083e58753d";
            var reloadedEvents = await _eventStore.GetEventsAsync(Guid.Parse(orderId));
            return Ok(reloadedEvents);
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder()
        {
            var orderId = "68230e7445b6c822d6a79257";
            var order = await _readOrderService.GetByIdAsync(orderId);


            return Ok(order);
        }

        [HttpGet("Test-Domain-Event")]
        public async Task<IActionResult> TestDomainEvent(CancellationToken token)
        {
            await eventBus.PublishAsync(new OrderCreatedDomainEvent(new ObjectId().ToString(), new ObjectId().ToString(), 3000, DateTime.UtcNow), token);

            return Ok();
        }

    }
}
