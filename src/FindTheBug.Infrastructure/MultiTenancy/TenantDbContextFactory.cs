using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FindTheBug.Infrastructure.MultiTenancy;

public class TenantDbContextFactory(ITenantService tenantService, IMemoryCache cache) : ITenantDbContextFactory
{
    public IApplicationDbContext CreateDbContext(string tenantId)
    {
        var connectionString = GetConnectionString(tenantId);
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase($"FindTheBug_Tenant_{tenantId}");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }

    public async Task<IApplicationDbContext> CreateDbContextAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantService.GetTenantByIdAsync(tenantId, cancellationToken);
        
        if (tenant is null)
        {
            throw new InvalidOperationException($"Tenant with ID '{tenantId}' not found");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use tenant-specific in-memory database
        // In production, this would use the tenant's connection string to a real database
        if (!string.IsNullOrEmpty(tenant.ConnectionString))
        {
            // For now, use in-memory with tenant-specific database name
            optionsBuilder.UseInMemoryDatabase($"FindTheBug_Tenant_{tenant.Subdomain}");
        }
        else
        {
            optionsBuilder.UseInMemoryDatabase($"FindTheBug_Tenant_{tenantId}");
        }
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private string GetConnectionString(string tenantId)
    {
        // Cache connection strings for performance
        return cache.GetOrCreate($"tenant_connection_{tenantId}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            
            var tenant = tenantService.GetTenantByIdAsync(tenantId).GetAwaiter().GetResult();
            
            if (tenant is null)
            {
                throw new InvalidOperationException($"Tenant with ID '{tenantId}' not found");
            }

            return tenant.ConnectionString ?? $"FindTheBug_Tenant_{tenantId}";
        })!;
    }
}

