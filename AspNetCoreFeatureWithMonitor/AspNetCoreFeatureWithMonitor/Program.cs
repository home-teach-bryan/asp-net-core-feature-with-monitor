using AspNetCoreFeatureWithMonitor.ActionFilter;
using AspNetCoreFeatureWithMonitor.DbContext;
using AspNetCoreFeatureWithMonitor.Jwt;
using AspNetCoreFeatureWithMonitor.Middleware;
using AspNetCoreFeatureWithMonitor.Models;
using AspNetCoreFeatureWithMonitor.ServiceCollection;
using AspNetCoreFeatureWithMonitor.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

namespace AspNetCoreFeatureWithMonitor;

public class Program
{
    public static void Main(string[] args)
    {
        var excludePath = new List<string>
        {
            "metrics", "health", "swagger"
        };
        var builder = WebApplication.CreateBuilder(args);
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Filter.With(new CustomLogEventFilter(excludePath))
            .CreateLogger();
        try
        {
            // Add services to the container.
            builder.Services.AddControllers(option => { option.Filters.Add<ValidationModelActionFilter>(); });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<ApiBehaviorOptions>(item => item.SuppressModelStateInvalidFilter = true);
            // swagger document spec 
            builder.Services.AddCustomSwaggerGen();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<JwtTokenGenerator>();
            builder.Services.AddSingleton<TokenCounter>();

            // jwt authentication setting
            builder.Services.AddCustomJwtAuthentication(builder.Configuration);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // rate limit
            builder.Services.AddCustomRateLimiter();

            // health check
            builder.Services.AddCustomHealthCheck();

            // logging
            builder.Services.AddSerilog();

            // Db Context
            builder.Services.AddDbContext<ProductContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ProductConnectionString"));
            });

            builder.Services.AddCustomOpenTelemetry();

            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCustomHealthCheck();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapPrometheusScrapingEndpoint();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<HttpLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}