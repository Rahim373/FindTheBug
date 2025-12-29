using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Issued, Paid, Cancelled
    public string? PaymentMethod { get; set; } // Cash, Card, UPI, Insurance
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public LabReceipt Patient { get; set; } = null!;
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
