using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class TestResult : BaseAuditableEntity
{
    public Guid TestEntryId { get; set; }
    public Guid TestParameterId { get; set; }
    public string ResultValue { get; set; } = string.Empty;
    public bool IsAbnormal { get; set; }
    public DateTime? ResultDate { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public TestEntry TestEntry { get; set; } = null!;
    public TestParameter TestParameter { get; set; } = null!;
}
