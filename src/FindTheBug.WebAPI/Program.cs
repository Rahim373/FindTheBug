using FindTheBug.Application;
using FindTheBug.Infrastructure;
using FindTheBug.Infrastructure.MultiTenancy;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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

// Add Tenant Resolution Middleware (must be early in pipeline)
app.UseMiddleware<TenantResolutionMiddleware>();

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

app.Run();

