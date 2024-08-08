using System.Net;
using System.Text.Json;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Middleware
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message) { }
    }

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate request,
            ILogger<ErrorHandlingMiddleware> logger
        )
        {
            _next = request;
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var response = new ApiResponse<object>(["An unexpected error occurred."]);

            switch (exception)
            {
                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    response =
                        notFoundException.Message == ""
                            ? response
                            : new ApiResponse<object>([notFoundException.Message]);
                    break;
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    response =
                        badRequestException.Message == ""
                            ? response
                            : new ApiResponse<object>([badRequestException.Message]);
                    break;
                case InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    break;
                default:
                    _logger.LogError(exception, "An unhandled exception has occurred");
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
