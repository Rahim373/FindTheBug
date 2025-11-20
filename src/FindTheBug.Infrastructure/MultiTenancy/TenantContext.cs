using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Infrastructure.MultiTenancy;

public class TenantContext : ITenantContext
{
    private Tenant? _currentTenant;

    public string? TenantId => _currentTenant?.Id.ToString();
    
    public Tenant? CurrentTenant => _currentTenant;

    public void SetTenant(Tenant tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        _currentTenant = tenant;
    }

    public string? GetConnectionString()
    {
        return _currentTenant?.ConnectionString;
    }
}

