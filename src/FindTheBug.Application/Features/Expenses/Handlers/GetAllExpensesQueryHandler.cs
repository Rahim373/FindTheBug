using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;
using FindTheBug.Application.Features.Expenses.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Expenses.Handlers;

public class GetAllExpensesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllExpensesQuery, PaginatedExpenseListDto>
{
    public async Task<ErrorOr<PaginatedExpenseListDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Expense>().GetQueryable();

        // Filter by department
        if (!string.IsNullOrEmpty(request.Department))
        {
            query = query.Where(e => e.Department == request.Department);
        }

        // Search by note or reference number
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(e => 
                e.Note.ToLower().Contains(request.Search.ToLower()) ||
                (e.ReferenceNo != null && e.ReferenceNo.ToLower().Contains(request.Search.ToLower())));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.Date)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new ExpenseListItemDto
            {
                Id = e.Id,
                Date = e.Date,
                Note = e.Note,
                Amount = e.Amount,
                PaymentMethod = e.PaymentMethod,
                ReferenceNo = e.ReferenceNo,
                Department = e.Department
            })
            .ToListAsync(cancellationToken);

        return new PaginatedExpenseListDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}