using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestParameters.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Handlers;

public class CreateTestParameterCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateTestParameterCommand, TestParameter>
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
