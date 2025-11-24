using Services.Interfaces;

namespace MyStore.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            // استخرج userId من JWT
            var userIdClaim = context.User?.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                // غير مسجل دخول
                await _next(context);
                return;
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // بناء المفتاح permissionKey من Request path + method
            var path = context.Request.Path.Value?.Trim('/').Replace("/", ".") ?? "";
            var method = context.Request.Method;

            // مثال: Products/Add -> Products.Add
            string permissionKey = $"{path}.{GetActionNameFromMethod(method)}";

            // تحقق من الصلاحية
            var hasPermission = await permissionService.UserHasPermissionAsync(userId, permissionKey);

            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: You don't have permission.");
                return;
            }

            // السماح للطلب بالاستمرار
            await _next(context);
        }

        private string GetActionNameFromMethod(string method) => method switch
        {
            "GET" => "View",
            "POST" => "Add",
            "PUT" => "Edit",
            "DELETE" => "Delete",
            _ => method
        };
    }
}
