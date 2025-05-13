using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Application.Interfaces;
using Order.Application.Queries.Models;
using Order.Domain.Events.Domain;

namespace Order.Infrastructure.Comsumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedDomainEvent>
    {
        private readonly IReadOrderWriter _writer;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(IReadOrderWriter writer, ILogger<OrderCreatedConsumer> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedDomainEvent> context)
        {

            var readOrderModel = new OrderReadModel()
            {
                Id = context.Message.OrderId,
                CustomerId = context.Message.CustomerId,
                CreatedAt = context.Message.CreatedAt,
                TotalAmount = context.Message.TotalAmount,
            };

            _logger.LogInformation("Consume CreatedOrderDomainEvent success");

            await _writer.InsertAsync(readOrderModel);
        }
    }
}
