using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Billing.Invoices.DTOs;

namespace FindTheBug.Application.Features.Billing.Invoices.Commands;

public record CreateInvoiceCommand(
    Guid PatientId,
    List<Guid> TestEntryIds,
    decimal TotalAmount
) : ICommand<InvoiceResponseDto>;
