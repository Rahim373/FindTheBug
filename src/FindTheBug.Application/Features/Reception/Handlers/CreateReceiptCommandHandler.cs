using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Reception.Commands;
using FindTheBug.Application.Features.Reception.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Reception.Handlers;

public class CreateReceiptCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateReceiptCommand, ReceiptResponseDto>
{
    public async Task<ErrorOr<Result<ReceiptResponseDto>>> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
    {
        // Check if receipt with invoice number exists
        var existing = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .FirstOrDefaultAsync(r => r.InvoiceNumber == request.InvoiceNumber, cancellationToken);

        if (existing != null)
            return Error.Conflict("Receipt.InvoiceNumberExists", "Receipt with this invoice number already exists");

        // Validate diagnostic test IDs
        if (request.TestEntries.Any())
        {
            var testIds = await unitOfWork.Repository<DiagnosticTest>().GetQueryable()
                .Where(dt => request.TestEntries.Select(te => te.DiagnosticTestId).Contains(dt.Id) && dt.IsActive)
                .Select(dt => dt.Id)
                .ToListAsync(cancellationToken);

            var invalidIds = request.TestEntries.Select(te => te.DiagnosticTestId).Except(testIds).ToList();
            if (invalidIds.Any())
                return Error.Validation("Receipt.InvalidTests", "Invalid diagnostic test IDs provided");
        }

        var receipt = new LabReceipt
        {
            InvoiceNumber = request.InvoiceNumber,
            FullName = request.FullName,
            Age = request.Age,
            IsAgeYear = request.IsAgeYear,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            ReferredByDoctorId = request.ReferredByDoctorId,
            SubTotal = request.SubTotal,
            Total = request.Total,
            Discount = request.Discount,
            Due = request.Due,
            Balace = request.Balance,
            LabReceiptStatus = request.LabReceiptStatus,
            ReportDeliveryStatus = request.ReportDeliveryStatus
        };

        var created = await unitOfWork.Repository<LabReceipt>().AddAsync(receipt, cancellationToken);

        // Add test entries
        if (request.TestEntries.Any())
        {
            foreach (var testEntry in request.TestEntries)
            {
                var receiptTest = new ReceiptTest
                {
                    LabReceiptId = created.Id,
                    DiagnosticTestId = testEntry.DiagnosticTestId,
                    Total = testEntry.Total,
                    Status = ReceiptTestStatus.Pending
                };
                await unitOfWork.Repository<ReceiptTest>().AddAsync(receiptTest, cancellationToken);
            }
        }

        // Get the created receipt with related data
        var receiptWithDetails = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .Include(r => r.ReferredBy)
            .Include(r => r.TestEntries)
                .ThenInclude(te => te.DiagnosticTest)
            .FirstAsync(r => r.Id == created.Id, cancellationToken);

        var receiptDto = new ReceiptResponseDto
        {
            Id = receiptWithDetails.Id,
            InvoiceNumber = receiptWithDetails.InvoiceNumber,
            FullName = receiptWithDetails.FullName,
            Age = receiptWithDetails.Age,
            IsAgeYear = receiptWithDetails.IsAgeYear,
            Gender = receiptWithDetails.Gender,
            PhoneNumber = receiptWithDetails.PhoneNumber,
            Address = receiptWithDetails.Address,
            ReferredByDoctorId = receiptWithDetails.ReferredByDoctorId,
            ReferredByDoctorName = receiptWithDetails.ReferredBy?.Name,
            SubTotal = receiptWithDetails.SubTotal,
            Total = receiptWithDetails.Total,
            Discount = receiptWithDetails.Discount,
            Due = receiptWithDetails.Due,
            Balance = receiptWithDetails.Balace,
            ReportDeliveredOn = receiptWithDetails.ReportDeliveredOn,
            LabReceiptStatus = receiptWithDetails.LabReceiptStatus,
            ReportDeliveryStatus = receiptWithDetails.ReportDeliveryStatus,
            TestEntries = receiptWithDetails.TestEntries.Select(te => new ReceiptTestDto
            {
                Id = te.Id,
                DiagnosticTestId = te.DiagnosticTestId,
                DiagnosticTestName = te.DiagnosticTest?.TestName ?? string.Empty,
                Total = te.Total,
                Status = (int)te.Status,
                StatusDisplay = te.Status.ToString()
            }).ToList(),
            CreatedAt = receiptWithDetails.CreatedAt,
            UpdatedAt = receiptWithDetails.UpdatedAt,
            CreatedBy = receiptWithDetails.CreatedBy,
            UpdatedBy = receiptWithDetails.UpdatedBy
        };

        return Result<ReceiptResponseDto>.Success(receiptDto);
    }
}