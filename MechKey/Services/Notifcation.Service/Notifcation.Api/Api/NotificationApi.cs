namespace Notifcation.Api
{
    public static class NotificationApi
    {
        public static IEndpointRouteBuilder MapNotificationApi(this IEndpointRouteBuilder builder)
        {
            builder.MapGroup("/api/v1/notification")
                  .MapNotificationApi();

            return builder;
        }

        public static RouteGroupBuilder MapNotificationApi(this RouteGroupBuilder group)
        {
            group.MapGet("health-check", () =>
            {
                return Results.Ok("Health-check called");
            });

            return group;
        }
    }
}
