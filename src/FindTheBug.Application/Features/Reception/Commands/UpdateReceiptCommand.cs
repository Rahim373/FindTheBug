using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Reception.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Reception.Commands;

public record UpdateReceiptCommand(
    Guid Id,
    string InvoiceNumber,
    string FullName,
    int? Age,
    bool IsAgeYear,
    string Gender,
    string PhoneNumber,
    string? Address,
    Guid? ReferredByDoctorId,
    decimal SubTotal,
    decimal Total,
    decimal Discount,
    decimal Due,
    decimal Balance,
    DateTime? ReportDeliveredOn,
    LabReceiptStatus LabReceiptStatus,
    ReportDeliveryStatus ReportDeliveryStatus,
    List<ReceiptTestEntryDto> TestEntries
) : ICommand<ReceiptResponseDto>;