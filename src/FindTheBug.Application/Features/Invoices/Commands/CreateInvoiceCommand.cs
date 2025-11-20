using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(
    Guid PatientId,
    List<InvoiceItemDto> Items
) : IRequest<Result<Invoice>>;

public record InvoiceItemDto(
    Guid? TestEntryId,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage
);

public class CreateInvoiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, Result<Invoice>>
{
    public async Task<Result<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                TenantId = string.Empty, // Will be set by DbContext
                PatientId = request.PatientId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                InvoiceDate = DateTime.UtcNow,
                Status = "Draft"
            };

            decimal subTotal = 0;
            foreach (var itemDto in request.Items)
            {
                var item = new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    TestEntryId = itemDto.TestEntryId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Amount = itemDto.Quantity * itemDto.UnitPrice,
                    DiscountPercentage = itemDto.DiscountPercentage,
                    DiscountAmount = (itemDto.Quantity * itemDto.UnitPrice) * (itemDto.DiscountPercentage / 100)
                };
                
                subTotal += item.Amount - item.DiscountAmount;
                await unitOfWork.Repository<InvoiceItem>().AddAsync(item, cancellationToken);
            }

            invoice.SubTotal = subTotal;
            invoice.TaxAmount = subTotal * 0.05m; // 5% tax
            invoice.TotalAmount = subTotal + invoice.TaxAmount;

            var created = await unitOfWork.Repository<Invoice>().AddAsync(invoice, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Invoice>.Success(created);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<Invoice>.Failure($"Error creating invoice: {ex.Message}");
        }
    }
}
