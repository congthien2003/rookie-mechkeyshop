using Application.Events;
using Application.Interfaces.IApiClient.Supabase;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient.Consumer
{
    public class DeleteImageConsumer : IConsumer<DeleteImageEvent>
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILogger<DeleteImageConsumer> _logger;

        public DeleteImageConsumer(ISupabaseService supabaseService, ILogger<DeleteImageConsumer> logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteImageEvent> context)
        {
            var result = await _supabaseService.DeleteImage(context.Message.Url);

            if (!result)
                throw new Exception("Handle delete image on cloud failed");

            _logger.LogInformation("Handle delete image on cloud success !!");

        }
    }
}
