using Application.Events;
using Application.Interfaces.IServices;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient.Consumer
{
    public class RequestEmailConsumer : IConsumer<RegisterSuccessEvent>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<RequestEmailConsumer> logger;

        public RequestEmailConsumer(IEmailService emailService, ILogger<RequestEmailConsumer> logger)
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
