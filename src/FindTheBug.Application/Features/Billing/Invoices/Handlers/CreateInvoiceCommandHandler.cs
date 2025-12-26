using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Billing.Invoices.Commands;
using FindTheBug.Application.Features.Billing.Invoices.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Billing.Invoices.Handlers;

public class CreateInvoiceCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateInvoiceCommand, InvoiceResponseDto>
{
    public async Task<ErrorOr<Result<InvoiceResponseDto>>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
            PatientId = request.PatientId,
            TotalAmount = request.TotalAmount,
            InvoiceDate = DateTime.UtcNow
        };

        var created = await unitOfWork.Repository<Invoice>().AddAsync(invoice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(created.PatientId, cancellationToken);

        return Result<InvoiceResponseDto>.Success(new InvoiceResponseDto
        {
            Id = created.Id,
            InvoiceNumber = created.InvoiceNumber,
            PatientId = created.PatientId,
            PatientName = patient?.FirstName ?? string.Empty,
            TotalAmount = created.TotalAmount,
            InvoiceDate = created.InvoiceDate,
            CreatedAt = created.CreatedAt
        });
    }
}
