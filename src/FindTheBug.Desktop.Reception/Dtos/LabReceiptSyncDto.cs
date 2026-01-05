namespace FindTheBug.Desktop.Reception.Dtos;

/// <summary>
/// DTO for syncing LabReceipt from Desktop to API
/// </summary>
public class LabReceiptSyncDto
{
    public Guid Id { get; set; }
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
    public decimal Balance { get; set; }

    public DateTime? ReportDeliveredOn { get; set; }
    public int LabReceiptStatus { get; set; }
    public int ReportDeliveryStatus { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public List<ReceiptTestSyncDto> TestEntries { get; set; } = new();
}