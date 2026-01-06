using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Reception.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Reception.Handlers;

public class DeleteReceiptCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteReceiptCommand, bool>
{
    public async Task<ErrorOr<Result<bool>>> Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
    {
        var receipt = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (receipt == null)
        {
            return Error.NotFound(
                code: "Receipt.NotFound",
                description: $"Receipt with ID {request.Id} was not found");
        }

        // Delete test entries first
        var testEntries = await unitOfWork.Repository<ReceiptTest>().GetQueryable()
            .Where(rt => rt.LabReceiptId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var testEntry in testEntries)
        {
            await unitOfWork.Repository<ReceiptTest>().DeleteAsync(testEntry.Id, cancellationToken);
        }

        // Delete receipt
        await unitOfWork.Repository<LabReceipt>().DeleteAsync(request.Id, cancellationToken);

        return Result<bool>.Success(true);
    }
}