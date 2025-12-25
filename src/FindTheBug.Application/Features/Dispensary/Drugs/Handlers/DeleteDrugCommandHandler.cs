using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Drugs.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class DeleteDrugCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteDrugCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteDrugCommand request, CancellationToken cancellationToken)
    {
        var drug = await unitOfWork.Repository<Drug>().GetQueryable()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (drug == null)
            return Error.NotFound("Drug.NotFound", "Drug not found");

        await unitOfWork.Repository<Drug>().DeleteAsync(drug.Id, cancellationToken);

        return true;
    }
}
