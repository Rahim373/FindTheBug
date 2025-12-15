using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Billing.Invoices.Queries;

public record GenerateInvoicePdfQuery(Guid InvoiceId) : IQuery<byte[]>;
