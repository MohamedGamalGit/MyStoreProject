using Commen.ViewModels;
using MyStore.Exceptions;
using Serilog;

namespace MyStore.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Trace ID from Serilog / HttpContext
            var traceId = Guid.NewGuid().ToString();

            ApiErrorResponse response;

            if (ex is AppException appEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                response = new ApiErrorResponse
                {
                    ErrorCode = appEx.ErrorCode,
                    ErrorMessage = appEx.Message,
                    TraceId = traceId
                };
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                response = new ApiErrorResponse
                {
                    ErrorCode = "SERVER_ERROR",
                    ErrorMessage = "Something went wrong!",
                    TraceId = traceId
                };
            }

            // Log the error with full metadata
            Log.Error(ex, "Error Occurred. TraceId: {TraceId}", traceId);

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
