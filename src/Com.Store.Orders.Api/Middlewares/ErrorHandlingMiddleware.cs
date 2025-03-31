using Com.Store.Orders.Domain.Services.Exceptions;

namespace Com.Store.Orders.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException exception)
            {
                await HandleDomainException(context, exception);
            }
            catch (Exception exception)
            {
                await HandleException(context, exception);
            }
        }

        private async Task HandleDomainException(HttpContext context, DomainException exception)
        {
            await WriteErrorMessage(context, exception.ErrorMessage, exception.ErrorCode);

            if (exception.ErrorCode < 500)
            {
                _logger.LogWarning(exception, exception.ErrorMessage);
            }
            else
            {
                _logger.LogError(exception, exception.ErrorMessage);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            await WriteErrorMessage(context, exception.Message, StatusCodes.Status500InternalServerError);

            _logger.LogError(exception, exception.Message);
        }

        private static async Task WriteErrorMessage(HttpContext context, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                Message = message
            });
        }
    }
}
