namespace FindTheBug.Application.Features.Reception.DTOs;

/// <summary>
/// DTO for receipt test entry in create/update requests
/// </summary>
public class ReceiptTestEntryDto
{
    public Guid DiagnosticTestId { get; set; }
    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}