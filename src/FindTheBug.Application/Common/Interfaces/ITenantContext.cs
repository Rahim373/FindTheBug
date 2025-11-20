using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Common.Interfaces;

public interface ITenantContext
{
    string? TenantId { get; }
    Tenant? CurrentTenant { get; }
    void SetTenant(Tenant tenant);
    string? GetConnectionString();
}
