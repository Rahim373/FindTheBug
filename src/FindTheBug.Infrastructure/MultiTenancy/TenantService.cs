using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.MultiTenancy;

public class TenantService(Data.MasterDbContext masterDbContext) : ITenantService
{
    public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await masterDbContext.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive, cancellationToken);
    }

    public async Task<Tenant?> GetTenantByIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(tenantId, out var id))
            return null;

        return await masterDbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await masterDbContext.Tenants
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.Id = Guid.NewGuid();
        await masterDbContext.Tenants.AddAsync(tenant, cancellationToken);
        await masterDbContext.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        masterDbContext.Tenants.Update(tenant);
        await masterDbContext.SaveChangesAsync(cancellationToken);
    }
}

