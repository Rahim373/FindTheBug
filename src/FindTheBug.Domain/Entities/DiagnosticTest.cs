using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class DiagnosticTest : BaseAuditableEntity, ITenantEntity
{
    public required string TenantId { get; set; }
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInHours { get; set; }
    public bool RequiresFasting { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<TestParameter> Parameters { get; set; } = new List<TestParameter>();
    public ICollection<TestEntry> TestEntries { get; set; } = new List<TestEntry>();
}
