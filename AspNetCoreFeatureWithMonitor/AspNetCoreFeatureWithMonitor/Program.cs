using AspNetCoreFeatureWithMonitor.ActionFilter;
using AspNetCoreFeatureWithMonitor.DbContext;
using AspNetCoreFeatureWithMonitor.Interceptor;
using AspNetCoreFeatureWithMonitor.Jwt;
using AspNetCoreFeatureWithMonitor.Middleware;
using AspNetCoreFeatureWithMonitor.Models;
using AspNetCoreFeatureWithMonitor.ServiceCollection;
using AspNetCoreFeatureWithMonitor.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCoreFeatureWithMonitor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(option =>
        {
            option.Filters.Add<ValidationModelActionFilter>();
            option.Filters.Add<ApiResponseActionFilter>();
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.Configure<ApiBehaviorOptions>(item => item.SuppressModelStateInvalidFilter = true);
        // swagger document spec 
        builder.Services.AddCustomSwaggerGen();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<JwtTokenGenerator>();
        
        // jwt authentication setting
        builder.Services.AddCustomJwtAuthentication(builder.Configuration);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // rate limit
        builder.Services.AddCustomRateLimiter();

        // health check
        builder.Services.AddCustomHealthCheck();
        // http logging
        builder.Services.AddCustomHttpLogging();
        builder.Services.AddHttpLoggingInterceptor<HttpLoggingInterceptor>();
        
        // Db Context
        builder.Services.AddDbContext<EFcoreSampleContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreSampleConnectionString"));
        });
        
        
        var app = builder.Build();
        app.UseMiddleware<ExceptionMiddleware>();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = (httpContext, healthReport) =>
            {
                var result = new
                {
                    health = Enum.Parse<HealthStatus>(healthReport.Status.ToString()).ToString(),
                    services = healthReport.Entries.Select(item => new
                    {
                        name = item.Key,
                        health = Enum.Parse<HealthStatus>(item.Value.Status.ToString()).ToString()
                    })
                };
                httpContext.Response.ContentType = "application/json";
                var json = System.Text.Json.JsonSerializer.Serialize(result);
                return httpContext.Response.WriteAsync(json);
            }
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseHttpLogging();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();

        app.MapControllers();
        app.Run();
    }
}