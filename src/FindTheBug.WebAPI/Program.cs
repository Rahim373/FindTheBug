using FindTheBug.WebAPI.Middleware;
using FindTheBug.WebAPI.Installers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using FindTheBug.Infrastructure.MultiTenancy;

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


// Add services to the container using Installers
builder.Services.InstallServicesInAssembly(builder.Configuration);

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

// Add Authentication and Authorization
app.UseAuthentication();
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

