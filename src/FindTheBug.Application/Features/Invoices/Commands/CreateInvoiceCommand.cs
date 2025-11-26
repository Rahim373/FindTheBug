using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Invoices.Contracts;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(
    Guid PatientId,
    List<InvoiceItemDto> Items,
    decimal? DiscountAmount,
    decimal? TaxAmount,
    string? PaymentMethod,
    string? Notes
) : ICommand<Invoice>;
