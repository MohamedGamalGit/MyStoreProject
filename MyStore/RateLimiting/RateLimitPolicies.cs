using System.Security.Claims;
using System.Threading.RateLimiting;

namespace MyStore.RateLimiting
{
    public static class RateLimitPolicies
    {
        public const string PerIp = "PerIp";
        public const string PerUser = "PerUser";

        public static RateLimitPartition<string> IpPolicy(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ip,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromSeconds(1),
                    QueueLimit = 0
                });
        }

        public static RateLimitPartition<string> UserPolicy(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RateLimitPartition.GetNoLimiter("anonymous");
            }

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: userId,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromSeconds(60),
                    QueueLimit = 0
                });
        }
        public static RateLimitPartition<string> IpAndUserPolicy(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            var key = $"{ip}-{userId}";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: key,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,                  // عدد requests المسموح بها
                    Window = TimeSpan.FromSeconds(10), // كل 10 ثواني
                    QueueLimit = 0
                });
        }

    }
}
