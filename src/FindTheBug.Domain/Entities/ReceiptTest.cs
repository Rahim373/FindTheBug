using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public enum ReceiptTestStatus
{
    Pending, 
    InProgress,
    ResultEntered,
    Completed
}

public class ReceiptTest : BaseAuditableEntity
{
    public Guid LabReceiptId { get; set; }
    public Guid DiagnosticTestId { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Total { get; set; }
    public ReceiptTestStatus Status { get; set; }

    // Navigation properties
    public LabReceipt Patient { get; set; } = null!;
    public DiagnosticTest DiagnosticTest { get; set; } = null!;
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
