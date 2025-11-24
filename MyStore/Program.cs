using Commen.Helpers;
using Infrastructure.Data;

using Microsoft.IdentityModel.Tokens;
using Repositories.GenericRepository;
using Repositories.IGenericRepository;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Services.Interfaces;
using Services.Services;
using Services.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MyStore.Middlewares;
using System.Text.Json;
using Models.Seed;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL ??? Angular app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ------------------- JWT -------------------
// JWT Middleware
builder.AddJwtAuthentication();

// Register JwtHelper from common project
builder.Services.AddSingleton(new JwtHelper(
    builder.Configuration["Jwt:Key"],
    builder.Configuration["Jwt:Issuer"],
    builder.Configuration["Jwt:Audience"],
    double.Parse(builder.Configuration["Jwt:ExpireMinutes"])
));

// ------------------- DbContext -------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// ------------------- Generic Repository -------------------
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // MappingProfile ?? ??? Profile class ?????
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// ------------------- Controllers -------------------
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
}); ;

// ------------------- Swagger / OpenAPI -------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------- Build & Middleware -------------------
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    SeedData.Seed(db);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyStore API V1");
        c.RoutePrefix = string.Empty; // ??? ???? Swagger ?? root URL
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
// Authentication must come before Authorization
app.UseJwtAuthentication();

app.MapControllers();

app.Run();
