namespace WebAPI.Extensions.Middlewares
{
    public class LogginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogginMiddleware> _logger;

        public LogginMiddleware(RequestDelegate next, ILogger<LogginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString(); // hoặc dùng context.TraceIdentifier
            var routeData = context.GetRouteData();

            var controller = routeData.Values["controller"]?.ToString() ?? "UnknownController";
            var action = routeData.Values["action"]?.ToString() ?? "UnknownAction";

            var scope = new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["Controller"] = controller,
                ["Action"] = action
            };

            using (_logger.BeginScope(scope))
            {
                await _next(context);
            }
        }
    }
}
