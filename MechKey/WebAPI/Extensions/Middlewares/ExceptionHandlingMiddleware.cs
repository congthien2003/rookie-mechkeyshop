using Application.Comoon;
using System.Net;

namespace WebAPI.Extensions.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = ex.Message;
            switch (ex)
            {
                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case NullReferenceException _:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case InvalidDataException _:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case Exception:
                    message = "An unexpected error occurred.";
                    break;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsJsonAsync(Result.Failure(message));
        }
    }
}