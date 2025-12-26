using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Expenses.Commands;
using FindTheBug.Application.Features.Expenses.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Expenses.Handlers;

public class UpdateExpenseCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateExpenseCommand, ExpenseResponseDto>
{
    public async Task<ErrorOr<ExpenseResponseDto>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await unitOfWork.Repository<Expense>().GetQueryable()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null)
            return Error.NotFound("Expense.NotFound", "Expense not found");

        expense.Date = request.Date;
        expense.Note = request.Note;
        expense.Amount = request.Amount;
        expense.PaymentMethod = request.PaymentMethod;
        expense.ReferenceNo = request.ReferenceNo;
        expense.Attachment = request.Attachment;
        expense.Department = request.Department;

        await unitOfWork.Repository<Expense>().UpdateAsync(expense, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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