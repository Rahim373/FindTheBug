using FindTheBug.Application;
using FindTheBug.Infrastructure;
using FindTheBug.Infrastructure.MultiTenancy;
using FindTheBug.WebAPI.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FindTheBug.WebAPI.Filters.ErrorOrActionFilter>();
});
builder.Services.AddHttpContextAccessor();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Request Logging Middleware (early in pipeline)
app.UseMiddleware<RequestLoggingMiddleware>();

// Add Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Add Tenant Resolution Middleware (must be early in pipeline)
app.UseMiddleware<TenantResolutionMiddleware>();

// Add Result Wrapper Middleware (wraps all responses in Result class)
app.UseMiddleware<ResultWrapperMiddleware>();

// Add HTTP metrics middleware (tracks request count, duration, etc.)
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

// Map Prometheus metrics endpoint
app.MapMetrics();

// Map health checks endpoint with UI formatting
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

try
{
    Log.Information("Starting FindTheBug application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

