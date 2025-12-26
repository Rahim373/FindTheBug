using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;

namespace FindTheBug.Application.Features.Expenses.Commands;

public record UpdateExpenseCommand(
    Guid Id,
    DateTime Date,
    string Note,
    decimal Amount,
    string PaymentMethod,
    string? ReferenceNo,
    string? Attachment,
    string Department
) : ICommand<ExpenseResponseDto>;