namespace FindTheBug.Application.Common.Interfaces;

public interface ITenantDbContextFactory
{
    IApplicationDbContext CreateDbContext(string tenantId);
    Task<IApplicationDbContext> CreateDbContextAsync(string tenantId, CancellationToken cancellationToken = default);
}

