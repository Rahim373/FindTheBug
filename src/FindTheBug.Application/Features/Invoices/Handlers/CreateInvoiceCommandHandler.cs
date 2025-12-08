using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Invoices.Commands;
using FindTheBug.Application.Features.Invoices.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Invoices.Handlers;

public class CreateInvoiceCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateInvoiceCommand, InvoiceResponseDto>
{
    public async Task<ErrorOr<InvoiceResponseDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
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

        return new InvoiceResponseDto
        {
            Id = created.Id,
            InvoiceNumber = created.InvoiceNumber,
            PatientId = created.PatientId,
            PatientName = patient?.FirstName ?? string.Empty,
            TotalAmount = created.TotalAmount,
            InvoiceDate = created.InvoiceDate,
            CreatedAt = created.CreatedAt
        };
    }
}
