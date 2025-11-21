using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public record UpdateTestParameterCommand(
    Guid Id,
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
) : IRequest<ErrorOr<TestParameter>>;

public class UpdateTestParameterCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateTestParameterCommand, ErrorOr<TestParameter>>
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