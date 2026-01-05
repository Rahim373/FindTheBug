using FindTheBug.Domain.Common;
using System.ComponentModel;

namespace FindTheBug.Domain.Entities;

public enum LabReceiptStatus
{
    Pending = 0,
    Paid = 1,
    Due = 2,
    Void = 3
}

public enum ReportDeliveryStatus
{
    [Description]
    NotDelivered = 0,
    Delivered = 1
}

public class LabReceipt : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public bool IsAgeYear { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public Guid? ReferredByDoctorId { get; set; }
    
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal Due { get; set; }
    public decimal Balace { get; set; }

    public DateTime? ReportDeliveredOn { get; set; }

    public LabReceiptStatus LabReceiptStatus { get; set; } = LabReceiptStatus.Pending;
    public ReportDeliveryStatus ReportDeliveryStatus { get; set; } = ReportDeliveryStatus.NotDelivered;

    
    // Navigation properties
    public ICollection<ReceiptTest> TestEntries { get; set; } = new List<ReceiptTest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public Doctor? ReferredBy { get; set; }
}
