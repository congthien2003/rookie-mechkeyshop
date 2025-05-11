using Constracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application;

namespace Notification.Consumer
{
    public class RegisterSuccessConsumer : IConsumer<RegisterSuccessEvent>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<RegisterSuccessConsumer> logger;

        public RegisterSuccessConsumer(IEmailService emailService, ILogger<RegisterSuccessConsumer> logger)
        {
            this.emailService = emailService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<RegisterSuccessEvent> context)
        {
            // Call email sender
            var result = emailService.SendEmailConfirm(context.Message.Email, context.Message.UserId.ToString());
            if (!result)
            {
                logger.LogError($"Failed to send confirmation email to {context.Message.Email}");
                throw new InvalidOperationException($"Failed to send confirmation email to {context.Message.Email}");
            }

            logger.LogInformation("Consume event success !!");


            return Task.CompletedTask;
        }
    }
}
