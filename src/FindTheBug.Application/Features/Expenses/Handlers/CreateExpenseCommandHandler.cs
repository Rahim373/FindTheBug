using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Expenses.Commands;
using FindTheBug.Application.Features.Expenses.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Expenses.Handlers;

public class CreateExpenseCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateExpenseCommand, ExpenseResponseDto>
{
    public async Task<ErrorOr<Result<ExpenseResponseDto>>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = new Expense
        {
            Date = request.Date,
            Note = request.Note,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            ReferenceNo = request.ReferenceNo,
            Attachment = request.Attachment,
            Department = request.Department
        };

        var created = await unitOfWork.Repository<Expense>().AddAsync(expense, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponseDto>.Success(new ExpenseResponseDto
        {
            Id = created.Id,
            Date = created.Date,
            Note = created.Note,
            Amount = created.Amount,
            PaymentMethod = created.PaymentMethod,
            ReferenceNo = created.ReferenceNo,
            Attachment = created.Attachment,
            Department = created.Department,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        });
    }
}