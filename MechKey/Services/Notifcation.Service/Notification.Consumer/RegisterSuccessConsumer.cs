using Constracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Core.Interfaces;
using Notification.Domain.Enum;

namespace Notification.Consumer
{
    public class RegisterSuccessConsumer : IConsumer<RegisterSuccessEvent>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<RegisterSuccessConsumer> logger;
        private readonly INotificationService notificationService;


        public RegisterSuccessConsumer(IEmailService emailService, ILogger<RegisterSuccessConsumer> logger, INotificationService notificationService)
        {
            this.emailService = emailService;
            this.logger = logger;
            this.notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<RegisterSuccessEvent> context)
        {
            var email = context.Message.Email;
            var userId = context.Message.UserId.ToString();
            var eventId = context.Message.Id.ToString();

            var notification = new Domain.Entities.Notification
            {
                Type = NotificationType.Email,
                Description = $"Send confirmation email to {email}",
                EventId = eventId,
                UserId = userId,
                Recipient = email,
                SendAt = DateTime.UtcNow
            };

            try
            {
                var result = emailService.SendEmailConfirm(email, userId);

                if (!result)
                {
                    notification.ChangeStatus(NotificationStatus.Failed);
                    logger.LogError("Failed to send confirmation email to {Email}", email);
                }
                else
                {
                    notification.ChangeStatus(NotificationStatus.Success);
                    logger.LogInformation("Confirmation email sent successfully to {Email}", email);
                }
            }
            catch (Exception ex)
            {
                notification.ChangeStatus(NotificationStatus.Failed);
                logger.LogError(ex, "Exception while sending confirmation email to {Email}", email);
            }

            await notificationService.LogNotificationAsync(notification);
        }

    }
}
