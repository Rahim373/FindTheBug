namespace FindTheBug.Application.Features.Laboratory.LabReceipts.DTOs;

/// <summary>
/// DTO for syncing LabReceipt with LabTests from Desktop application
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

/// <summary>
/// DTO for syncing ReceiptTest from Desktop application
/// </summary>
public class ReceiptTestSyncDto
{
    public Guid Id { get; set; }
    public Guid LabReceiptId { get; set; }
    public Guid DiagnosticTestId { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Total { get; set; }
    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}