using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestParameters.Commands;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Handlers;

public class UpdateTestParameterCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTestParameterCommand, TestParameterResponseDto>
{
    public async Task<ErrorOr<TestParameterResponseDto>> Handle(UpdateTestParameterCommand request, CancellationToken cancellationToken)
    {
        var parameter = await unitOfWork.Repository<TestParameter>().GetByIdAsync(request.Id, cancellationToken);

        if (parameter == null)
            return Error.NotFound("TestParameter.NotFound", "Test parameter not found");

        parameter.ParameterName = request.ParameterName;
        parameter.Unit = request.Unit;
        parameter.ReferenceRangeMin = request.ReferenceRangeMin;
        parameter.ReferenceRangeMax = request.ReferenceRangeMax;
        parameter.DataType = request.DataType;
        parameter.DisplayOrder = request.DisplayOrder;

        await unitOfWork.Repository<TestParameter>().UpdateAsync(parameter);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TestParameterResponseDto
        {
            Id = parameter.Id,
            DiagnosticTestId = parameter.DiagnosticTestId,
            ParameterName = parameter.ParameterName,
            Unit = parameter.Unit,
            ReferenceRangeMin = parameter.ReferenceRangeMin,
            ReferenceRangeMax = parameter.ReferenceRangeMax,
            DataType = parameter.DataType,
            DisplayOrder = parameter.DisplayOrder,
            CreatedAt = parameter.CreatedAt
        };
    }
}
