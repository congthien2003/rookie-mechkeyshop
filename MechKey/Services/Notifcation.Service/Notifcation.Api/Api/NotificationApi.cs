using Notification.Application.Interfaces;

namespace Notifcation.Api
{
    public static class NotificationApi
    {
        public static IEndpointRouteBuilder MapNotificationApi(this IEndpointRouteBuilder builder)
        {
            builder.MapGroup("/api/v1/")
                  .MapNotificationApi();

            return builder;
        }

        public static RouteGroupBuilder MapNotificationApi(this RouteGroupBuilder group)
        {
            group.MapGet("health-check", () =>
            {
                return Results.Ok("Health-check called");
            });

            group.MapGet("notification", async (INotificationService service) =>
            {
                var list = await service.GetUserNotificationsAsync("6821707e55a7835ec5a12d70");
                return list;
            });

            group.MapGet("notification/{id:guid}", async (INotificationService service) =>
            {
                var list = await service.GetUserNotificationsAsync("123");
                return list;
            });

            return group;
        }
    }
}
