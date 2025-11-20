using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Invoices.Commands;

public record InvoiceItemDto(
    Guid TestEntryId,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal? DiscountPercentage
);

public record CreateInvoiceCommand(
    Guid PatientId,
    List<InvoiceItemDto> Items,
    decimal? DiscountAmount,
    decimal? TaxAmount,
    string? PaymentMethod,
    string? Notes
) : IRequest<ErrorOr<Invoice>>;

public class CreateInvoiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, ErrorOr<Invoice>>
{
    public async Task<ErrorOr<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = string.Empty, // Will be set by DbContext
            PatientId = request.PatientId,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            InvoiceDate = DateTime.UtcNow,
            Status = "Draft",
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes
        };

        var invoiceItems = request.Items.Select(item =>
        {
            var amount = item.Quantity * item.UnitPrice;
            var discountAmount = item.DiscountPercentage.HasValue
                ? amount * (item.DiscountPercentage.Value / 100)
                : 0;

            return new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                TestEntryId = item.TestEntryId,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Amount = amount,
                DiscountPercentage = item.DiscountPercentage.GetValueOrDefault(),
                DiscountAmount = discountAmount
            };
        }).ToList();

        var subTotal = invoiceItems.Sum(i => i.Amount - i.DiscountAmount);
        invoice.SubTotal = subTotal;
        invoice.DiscountAmount = request.DiscountAmount ?? 0;
        invoice.TaxAmount = request.TaxAmount ?? 0;
        invoice.TotalAmount = subTotal - invoice.DiscountAmount + invoice.TaxAmount;

        var createdInvoice = await unitOfWork.Repository<Invoice>().AddAsync(invoice, cancellationToken);

        foreach (var item in invoiceItems)
        {
            await unitOfWork.Repository<InvoiceItem>().AddAsync(item, cancellationToken);
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);

        return createdInvoice;
    }
}
