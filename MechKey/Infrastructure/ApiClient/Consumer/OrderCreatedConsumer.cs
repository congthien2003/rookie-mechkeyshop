using Application.Events;
using Application.Interfaces.IServices;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient.Consumer
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<OrderCreatedConsumer> logger;

        public OrderCreatedConsumer(IEmailService emailService, ILogger<OrderCreatedConsumer> logger)
        {
            this.emailService = emailService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            // Call email sender
            var result = emailService.SendEmailOrder(context.Message.OrderModel.Email, context.Message.OrderModel);
            if (!result)
            {
                logger.LogError($"Failed to send confirmation email to {context.Message.OrderModel.Email}");
                throw new InvalidOperationException($"Failed to send confirmation email to {context.Message.OrderModel.Email}");
            }

            logger.LogInformation($"Confirmation email sent successfully to {context.Message.OrderModel.Email}");


            return Task.CompletedTask;
        }
    }
}
