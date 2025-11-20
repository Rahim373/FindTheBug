using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? SubscriptionTier { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }
}

