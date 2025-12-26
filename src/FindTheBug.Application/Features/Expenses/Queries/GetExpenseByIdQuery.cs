using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;

namespace FindTheBug.Application.Features.Expenses.Queries;

public record GetExpenseByIdQuery(Guid Id) : IQuery<ExpenseResponseDto>;