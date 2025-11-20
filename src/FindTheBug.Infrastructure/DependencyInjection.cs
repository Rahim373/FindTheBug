using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Infrastructure.Data;
using FindTheBug.Infrastructure.MultiTenancy;
using FindTheBug.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            
            if (tenantContext?.TenantId != null)
            {
                return (ApplicationDbContext)factory.CreateDbContext(tenantContext.TenantId);
            }
            
            // Fallback to default in-memory database if no tenant context
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("FindTheBugDefaultDb");
            return new ApplicationDbContext(optionsBuilder.Options, tenantContext!);
        });

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
