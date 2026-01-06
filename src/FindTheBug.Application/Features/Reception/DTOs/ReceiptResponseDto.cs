using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Reception.DTOs;

/// <summary>
/// DTO for receipt details response
/// </summary>
public class ReceiptResponseDto
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
    public string? ReferredByDoctorName { get; set; }
    
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal Due { get; set; }
    public decimal Balance { get; set; }

    public DateTime? ReportDeliveredOn { get; set; }

    public LabReceiptStatus LabReceiptStatus { get; set; }
    public ReportDeliveryStatus ReportDeliveryStatus { get; set; }

    public List<ReceiptTestDto> TestEntries { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// DTO for receipt test entry
/// </summary>
public class ReceiptTestDto
{
    public Guid Id { get; set; }
    public Guid DiagnosticTestId { get; set; }
    public string DiagnosticTestName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public int Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
}