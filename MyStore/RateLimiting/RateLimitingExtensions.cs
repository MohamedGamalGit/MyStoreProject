using System.Security.Claims;
using System.Threading.RateLimiting;

namespace MyStore.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddAdvancedRateLimiting(
            this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var retryAfter =
                        context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ra)
                        ? ra.TotalSeconds
                        : 10;

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        code = 429,
                        message = "Too many requests. Please slow down.",
                        retryAfterSeconds = retryAfter,
                        traceId = context.HttpContext.TraceIdentifier
                    }, token);
                };
                options.AddPolicy("PerIpAndUser", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

                    // لو المستخدم anonymous → limit على IP بس
                    var key = userId == "anonymous" ? ip : $"{ip}-{userId}";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: key,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = userId == "anonymous" ? 5 : 10, // عدد requests
                            Window = userId == "anonymous" ? TimeSpan.FromSeconds(10) : TimeSpan.FromSeconds(10),
                            QueueLimit = 0
                        });
                });
                //options.AddPolicy(
                //    RateLimitPolicies.PerIp,
                //    context => RateLimitPolicies.IpPolicy(context));

                //options.AddPolicy(
                //    RateLimitPolicies.PerUser,
                //    context => RateLimitPolicies.UserPolicy(context));
            });

            return services;
        }

        public static IApplicationBuilder UseAdvancedRateLimiting(
            this IApplicationBuilder app)
        {
            app.UseRateLimiter();
            return app;
        }
    }
}
