using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.LabReceipts.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.LabReceipts.Commands;

/// <summary>
/// Command to sync LabReceipt with LabTests from Desktop application
/// </summary>
public record SyncLabReceiptCommand(LabReceiptSyncDto Request) : ICommand<string>;

/// <summary>
/// Handler for syncing LabReceipt with LabTests
/// </summary>
public class SyncLabReceiptCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<SyncLabReceiptCommand, string>
{
    public async Task<ErrorOr<Result<string>>> Handle(SyncLabReceiptCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;

        // Check if LabReceipt already exists
        var existingReceipt = await unitOfWork.Repository<LabReceipt>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.InvoiceNumber == dto.InvoiceNumber, cancellationToken);

        if (existingReceipt != null)
        {
            // Update existing LabReceipt
            existingReceipt.FullName = dto.FullName;
            existingReceipt.Age = dto.Age;
            existingReceipt.IsAgeYear = dto.IsAgeYear;
            existingReceipt.Gender = dto.Gender;
            existingReceipt.PhoneNumber = dto.PhoneNumber;
            existingReceipt.Address = dto.Address;
            existingReceipt.ReferredByDoctorId = dto.ReferredByDoctorId;
            existingReceipt.SubTotal = dto.SubTotal;
            existingReceipt.Total = dto.Total;
            existingReceipt.Discount = dto.Discount;
            existingReceipt.Due = dto.Due;
            existingReceipt.Balace = dto.Balance;
            existingReceipt.ReportDeliveredOn = dto.ReportDeliveredOn;
            existingReceipt.LabReceiptStatus = (LabReceiptStatus)dto.LabReceiptStatus;
            existingReceipt.ReportDeliveryStatus = (ReportDeliveryStatus)dto.ReportDeliveryStatus;
            existingReceipt.UpdatedBy = dto.UpdatedBy;
            existingReceipt.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Repository<LabReceipt>().UpdateAsync(existingReceipt, cancellationToken);

            // Handle TestEntries - remove all existing and add new ones
            var existingTests = await unitOfWork.Repository<ReceiptTest>()
                .GetQueryable()
                .Where(rt => rt.LabReceiptId == existingReceipt.Id)
                .ToListAsync(cancellationToken);

            foreach (var existingTest in existingTests)
            {
                await unitOfWork.Repository<ReceiptTest>().DeleteAsync(existingTest.Id, cancellationToken);
            }

            foreach (var testDto in dto.TestEntries)
            {
                var receiptTest = new ReceiptTest
                {
                    Id = testDto.Id,
                    LabReceiptId = existingReceipt.Id,
                    DiagnosticTestId = testDto.DiagnosticTestId,
                    Amount = testDto.Amount,
                    DiscountPercentage = testDto.DiscountPercentage,
                    Total = testDto.Total,
                    Status = (ReceiptTestStatus)testDto.Status,
                    CreatedAt = testDto.CreatedAt,
                    UpdatedAt = testDto.UpdatedAt,
                    CreatedBy = testDto.CreatedBy,
                    UpdatedBy = testDto.UpdatedBy
                };

                await unitOfWork.Repository<ReceiptTest>().AddAsync(receiptTest, cancellationToken);
            }
        }
        else
        {
            // Create new LabReceipt
            var newReceipt = new LabReceipt
            {
                Id = dto.Id,
                InvoiceNumber = dto.InvoiceNumber,
                FullName = dto.FullName,
                Age = dto.Age,
                IsAgeYear = dto.IsAgeYear,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                ReferredByDoctorId = dto.ReferredByDoctorId,
                SubTotal = dto.SubTotal,
                Total = dto.Total,
                Discount = dto.Discount,
                Due = dto.Due,
                Balace = dto.Balance,
                ReportDeliveredOn = dto.ReportDeliveredOn,
                LabReceiptStatus = (LabReceiptStatus)dto.LabReceiptStatus,
                ReportDeliveryStatus = (ReportDeliveryStatus)dto.ReportDeliveryStatus,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                CreatedBy = dto.CreatedBy,
                UpdatedBy = dto.UpdatedBy
            };

            await unitOfWork.Repository<LabReceipt>().AddAsync(newReceipt, cancellationToken);

            // Add TestEntries
            foreach (var testDto in dto.TestEntries)
            {
                var receiptTest = new ReceiptTest
                {
                    Id = testDto.Id,
                    LabReceiptId = newReceipt.Id,
                    DiagnosticTestId = testDto.DiagnosticTestId,
                    Amount = testDto.Amount,
                    DiscountPercentage = testDto.DiscountPercentage,
                    Total = testDto.Total,
                    Status = (ReceiptTestStatus)testDto.Status,
                    CreatedAt = testDto.CreatedAt,
                    UpdatedAt = testDto.UpdatedAt,
                    CreatedBy = testDto.CreatedBy,
                    UpdatedBy = testDto.UpdatedBy
                };

                await unitOfWork.Repository<ReceiptTest>().AddAsync(receiptTest, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success($"LabReceipt {dto.InvoiceNumber} synced successfully");
    }
}