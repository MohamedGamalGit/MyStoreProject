using Services.Interfaces;
using System.Diagnostics;
using System.Text;

namespace MyStore.Middlewares
{
    public class ActivityLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ActivityLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IActivityLogService logService)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var request = context.Request;

            // Get user info
            var userId = context.User?.FindFirst("UserId")?.Value;
            // UserName
            if (userId == null)
            {
                // Skip logging for unauthenticated requests (like login)
                await _next(context);
                return;
            }
            var ignoredPaths = new[]
            {
                "/api/Products/getProductsWithPagination",
                "/api/Categories/getCategoriesWithagination",
                "/api/Logs/activity-logs"
            };

            if (ignoredPaths.Any(p => request.Path.StartsWithSegments(p)) && request.Method == "POST")
            {
                await _next(context);
                return;
            }
            var userName = context.User?.FindFirst("UserName")?.Value;

            // الدور (Roles)
            var role = context.User?.FindAll("Roles")?.ToList();
            List<string> roleNames = new List<string>();
            if (role != null)
            {
                foreach (var r in role)
                {
                    roleNames.Add(r.Value);
                }
            }
            // Read request body (for POST/PUT)
            string bodyText = "";
            if (request.Method == "POST" || request.Method == "PUT")
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                bodyText = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            // Continue pipeline
            await _next(context);

            stopwatch.Stop();

            // After request
            var statusCode = context.Response.StatusCode;

            await logService.LogAsync(
                userId: userId,
                userName: userName,
                action: request.Method,
                entityName: request.Path.Value,
                entityId: null,
                description: bodyText,
                path: request.Path.Value,
                method: request.Method,
                ip: context.Connection.RemoteIpAddress?.ToString(),
                status: statusCode,
                executionTime: (int)stopwatch.Elapsed.TotalSeconds,
                roles: roleNames
            );
        }
    }
}
