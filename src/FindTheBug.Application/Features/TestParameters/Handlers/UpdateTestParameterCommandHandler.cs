using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestParameters.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Handlers;

public class UpdateTestParameterCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<UpdateTestParameterCommand, TestParameter>
{
    public async Task<ErrorOr<TestParameter>> Handle(UpdateTestParameterCommand request, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<TestParameter>().GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            return Error.NotFound("TestParameter.NotFound", $"Test parameter with ID {request.Id} not found");

        existing.ParameterName = request.ParameterName;
        existing.Unit = request.Unit;
        existing.ReferenceRangeMin = request.ReferenceRangeMin;
        existing.ReferenceRangeMax = request.ReferenceRangeMax;
        existing.DataType = request.DataType;
        existing.DisplayOrder = request.DisplayOrder;

        await unitOfWork.Repository<TestParameter>().UpdateAsync(existing, cancellationToken);
        return existing;
    }
}
