using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class SampleEntity : BaseAuditableEntity, ITenantEntity
{
    public required string TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

