using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Expenses.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Expenses.Handlers;

public class DeleteExpenseCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteExpenseCommand, bool>
{
    public async Task<ErrorOr<Result<bool>>> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await unitOfWork.Repository<Expense>().GetQueryable()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null)
            return Error.NotFound("Expense.NotFound", "Expense not found");

        await unitOfWork.Repository<Expense>().DeleteAsync(expense.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}