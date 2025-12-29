using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class InvoiceItem : BaseAuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Guid? TestEntryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }

    // Navigation properties
    public Invoice Invoice { get; set; } = null!;
    public ReceiptTest? TestEntry { get; set; }
}
