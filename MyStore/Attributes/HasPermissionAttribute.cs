using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace MyStore.Attributes
{
    public class HasPermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _permission;

        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = Guid.Parse(userIdClaim);

            // استدعاء service يتحقق من الصلاحية
            var svc = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            var hasPermission = await svc.UserHasPermissionAsync(userId, _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult(); // 403
                return;
            }

            await next();
        }
    }
}
