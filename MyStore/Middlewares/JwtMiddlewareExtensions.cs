using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyStore.Middlewares
{
    public static class JwtMiddlewareExtensions
    {
        public static void AddJwtAuthentication(this WebApplicationBuilder builder)
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void UseJwtAuthentication(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMiddleware<PermissionMiddleware>();
            app.UseMiddleware<ActivityLoggingMiddleware>();

        }
    }
}
