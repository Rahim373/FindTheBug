namespace FindTheBug.Application.Features.Reception.DTOs;

/// <summary>
/// DTO for list item in receipts list
/// </summary>
public class ReceiptListItemDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Due { get; set; }
    public int Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}