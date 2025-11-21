using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public record CreateTestParameterCommand(
    Guid DiagnosticTestId,
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
) : IRequest<ErrorOr<TestParameter>>;

public class CreateTestParameterCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateTestParameterCommand, ErrorOr<TestParameter>>
{
    public async Task<ErrorOr<TestParameter>> Handle(CreateTestParameterCommand request, CancellationToken cancellationToken)
    {
        var parameter = new TestParameter
        {
            Id = Guid.NewGuid(),
            TenantId = string.Empty,
            DiagnosticTestId = request.DiagnosticTestId,
            ParameterName = request.ParameterName,
            Unit = request.Unit,
            ReferenceRangeMin = request.ReferenceRangeMin,
            ReferenceRangeMax = request.ReferenceRangeMax,
            DataType = request.DataType,
            DisplayOrder = request.DisplayOrder
        };

        var created = await unitOfWork.Repository<TestParameter>().AddAsync(parameter, cancellationToken);
        return created;
    }
}