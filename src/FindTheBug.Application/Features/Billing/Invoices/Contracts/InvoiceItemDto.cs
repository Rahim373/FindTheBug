namespace FindTheBug.Application.Features.Billing.Invoices.Contracts;

public record InvoiceItemDto(
    Guid TestEntryId,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal? DiscountPercentage
);
