namespace FindTheBug.Application.Features.Invoices.DTOs;

public record InvoiceResponseDto
{
    public Guid Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime InvoiceDate { get; init; }
    public DateTime CreatedAt { get; init; }
}
