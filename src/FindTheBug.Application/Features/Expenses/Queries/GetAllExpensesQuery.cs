using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;

namespace FindTheBug.Application.Features.Expenses.Queries;

public record GetAllExpensesQuery(
    string? Department,
    string? Search,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PaginatedExpenseListDto>;