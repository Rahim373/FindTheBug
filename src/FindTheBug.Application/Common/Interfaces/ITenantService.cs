using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Common.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetTenantBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);
    Task<Tenant?> GetTenantByIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
    Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
