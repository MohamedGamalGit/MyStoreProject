using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Commen.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("UserId claim is missing from JWT token.");

            return Guid.Parse(userId);
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user?.FindFirst("UserName")?.Value ?? string.Empty;
        }
    }
}
