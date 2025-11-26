namespace FindTheBug.Application.Features.Invoices.Contracts;

public record InvoiceItemDto(
    Guid TestEntryId,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal? DiscountPercentage
);
