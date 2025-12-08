using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Invoices.DTOs;

namespace FindTheBug.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(
    Guid PatientId,
    List<Guid> TestEntryIds,
    decimal TotalAmount
) : ICommand<InvoiceResponseDto>;
