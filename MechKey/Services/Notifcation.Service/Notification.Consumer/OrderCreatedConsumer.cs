using Constracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;
using Notification.Domain.Enum;

namespace Notification.Consumer
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<OrderCreatedConsumer> logger;
        private readonly INotificationService notificationService;

        public OrderCreatedConsumer(IEmailService emailService,
                                    ILogger<OrderCreatedConsumer> logger,
                                    INotificationService notificationService)
        {
            this.emailService = emailService;
            this.logger = logger;
            this.notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var email = context.Message.OrderModel.Email;
            var userId = context.Message.OrderModel.UserId.ToString();
            var eventId = context.Message.Id.ToString();

            var notification = new Domain.Entities.Notification
            {
                Type = NotificationType.Email,
                Description = $"Send order confirmation email to {email}",
                EventId = eventId,
                UserId = userId,
                Recipient = email,
                SendAt = DateTime.UtcNow
            };

            try
            {
                var result = emailService.SendEmailOrder(email, context.Message.OrderModel);

                if (!result)
                {
                    notification.ChangeStatus(NotificationStatus.Failed);
                    logger.LogError("Failed to send order confirmation email to {Email}", email);
                }
                else
                {
                    notification.ChangeStatus(NotificationStatus.Success);
                    logger.LogInformation("Order confirmation email sent successfully to {Email}", email);
                }
            }
            catch (Exception ex)
            {
                notification.ChangeStatus(NotificationStatus.Failed);
                logger.LogError(ex, "Exception while sending order confirmation email to {Email}", email);
            }

            await notificationService.LogNotificationAsync(notification);
        }

    }
}
