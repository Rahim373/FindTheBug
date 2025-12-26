using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.DTOs;
using FindTheBug.Application.Features.Expenses.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Expenses.Handlers;

public class GetExpenseByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetExpenseByIdQuery, ExpenseResponseDto>
{
    public async Task<ErrorOr<ExpenseResponseDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var expense = await unitOfWork.Repository<Expense>().GetQueryable()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null)
            return Error.NotFound("Expense.NotFound", "Expense not found");

        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Date = expense.Date,
            Note = expense.Note,
            Amount = expense.Amount,
            PaymentMethod = expense.PaymentMethod,
            ReferenceNo = expense.ReferenceNo,
            Attachment = expense.Attachment,
            Department = expense.Department,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}