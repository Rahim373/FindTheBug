namespace FindTheBug.Desktop.Reception.Dtos;

/// <summary>
/// DTO for syncing ReceiptTest from Desktop to API
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