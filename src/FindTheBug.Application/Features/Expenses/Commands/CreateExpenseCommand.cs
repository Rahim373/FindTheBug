using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;

namespace FindTheBug.Application.Features.Expenses.Commands;

public record CreateExpenseCommand(
    DateTime Date,
    string Note,
    decimal Amount,
    string PaymentMethod,
    string? ReferenceNo,
    string? Attachment,
    string Department
) : ICommand<ExpenseResponseDto>;