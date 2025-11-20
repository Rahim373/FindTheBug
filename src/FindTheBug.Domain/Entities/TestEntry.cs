using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class TestEntry : BaseAuditableEntity, ITenantEntity
{
    public required string TenantId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DiagnosticTestId { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public DateTime? SampleCollectionDate { get; set; }
    public string Status { get; set; } = "Registered"; // Registered, SampleCollected, InProgress, Completed, Cancelled
    public string Priority { get; set; } = "Normal"; // Normal, Urgent, STAT
    public string? ReferredBy { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public DiagnosticTest DiagnosticTest { get; set; } = null!;
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
