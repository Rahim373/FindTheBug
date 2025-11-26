using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Infrastructure.Authentication;
using FindTheBug.Infrastructure.Data;
using FindTheBug.Infrastructure.Monitoring;
using FindTheBug.Infrastructure.MultiTenancy;
using FindTheBug.Infrastructure.Persistence;
using FindTheBug.Infrastructure.Repositories;
using FindTheBug.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace FindTheBug.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Master DbContext for tenant management (in-memory for now)
        services.AddDbContext<MasterDbContext>(options =>
            options.UseInMemoryDatabase("FindTheBugMasterDb"));

        // Add Multi-Tenancy services
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
        
        // Add memory cache for tenant connection string caching
        services.AddMemoryCache();

        // Add ApplicationDbContext - will be created per tenant via factory
        services.AddScoped<ApplicationDbContext>(provider =>
        {
            var tenantContext = provider.GetService<ITenantContext>();
            var factory = provider.GetRequiredService<ITenantDbContextFactory>();
            
            if (tenantContext?.TenantId is not null)
            {
                return (ApplicationDbContext)factory.CreateDbContext(tenantContext.TenantId);
            }
            
            // Fallback to default database if no tenant context
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options, tenantContext!);
        });

        // Register DbInitializer
        services.AddScoped<DbInitializer>();

        // Register repositories and Unit of Work
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register metrics service
        services.AddSingleton<IMetricsService, MetricsService>();
        
        // Register authentication services
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();

        // Add health checks
        services.AddHealthChecks()
            .AddCheck("master_database", () =>
            {
                // In-memory database is always healthy
                return HealthCheckResult.Healthy("Master database is healthy");
            })
            .AddCheck("application_health", () =>
            {
                return HealthCheckResult.Healthy("Application is running");
            });

        return services;
    }
}
