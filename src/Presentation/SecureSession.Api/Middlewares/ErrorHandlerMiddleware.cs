using SecureSession.Api.Extensions;
using SecureSession.Api.Items;
using System.Net;

namespace SecureSession.Api.Middlewares
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            var statusCode = HttpStatusCode.InternalServerError;

            if (ex is UnauthorizedAccessException)
                statusCode = HttpStatusCode.Unauthorized;
            else if (ex is KeyNotFoundException)
                statusCode = HttpStatusCode.NotFound;
            else if (ex is InvalidOperationException)
                statusCode = HttpStatusCode.BadRequest;
            else if (ex is ArgumentException)
                statusCode = HttpStatusCode.NotFound;
            else if (ex is ArgumentNullException)
                statusCode = HttpStatusCode.NotFound;

            var response = ResponseItem<object>.GenerateError(ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(response.Serialize() ?? "Uncaught Error");
        }
    }
}
