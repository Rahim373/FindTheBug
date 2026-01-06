using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Reception.DTOs;
using FindTheBug.Application.Features.Reception.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Reception.Handlers;

public class GetReceiptByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetReceiptByIdQuery, ReceiptResponseDto>
{
    public async Task<ErrorOr<Result<ReceiptResponseDto>>> Handle(GetReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .Include(r => r.ReferredBy)
            .Include(r => r.TestEntries)
                .ThenInclude(te => te.DiagnosticTest)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (receipt == null)
        {
            return Error.NotFound(
                code: "Receipt.NotFound",
                description: $"Receipt with ID {request.Id} was not found");
        }

        var receiptDto = new ReceiptResponseDto
        {
            Id = receipt.Id,
            InvoiceNumber = receipt.InvoiceNumber,
            FullName = receipt.FullName,
            Age = receipt.Age,
            IsAgeYear = receipt.IsAgeYear,
            Gender = receipt.Gender,
            PhoneNumber = receipt.PhoneNumber,
            Address = receipt.Address,
            ReferredByDoctorId = receipt.ReferredByDoctorId,
            ReferredByDoctorName = receipt.ReferredBy?.Name,
            SubTotal = receipt.SubTotal,
            Total = receipt.Total,
            Discount = receipt.Discount,
            Due = receipt.Due,
            Balance = receipt.Balace,
            ReportDeliveredOn = receipt.ReportDeliveredOn,
            LabReceiptStatus = receipt.LabReceiptStatus,
            ReportDeliveryStatus = receipt.ReportDeliveryStatus,
            TestEntries = receipt.TestEntries.Select(te => new ReceiptTestDto
            {
                Id = te.Id,
                DiagnosticTestId = te.DiagnosticTestId,
                DiagnosticTestName = te.DiagnosticTest?.TestName ?? string.Empty,
                Total = te.Total,
                Status = (int)te.Status,
                StatusDisplay = te.Status.ToString()
            }).ToList(),
            CreatedAt = receipt.CreatedAt,
            UpdatedAt = receipt.UpdatedAt,
            CreatedBy = receipt.CreatedBy,
            UpdatedBy = receipt.UpdatedBy
        };

        return Result<ReceiptResponseDto>.Success(receiptDto);
    }
}