using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Invoices.Queries;

public record GenerateInvoicePdfQuery(Guid InvoiceId) : IQuery<byte[]>;
