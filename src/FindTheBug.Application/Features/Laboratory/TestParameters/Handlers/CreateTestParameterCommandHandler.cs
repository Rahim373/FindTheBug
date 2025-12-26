using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.TestParameters.Commands;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Handlers;

public class CreateTestParameterCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTestParameterCommand, TestParameterResponseDto>
{
    public async Task<ErrorOr<Result<TestParameterResponseDto>>> Handle(CreateTestParameterCommand request, CancellationToken cancellationToken)
    {
        var parameter = new TestParameter
        {
            DiagnosticTestId = request.DiagnosticTestId,
            ParameterName = request.ParameterName,
            Unit = request.Unit,
            ReferenceRangeMin = request.ReferenceRangeMin,
            ReferenceRangeMax = request.ReferenceRangeMax,
            DataType = request.DataType,
            DisplayOrder = request.DisplayOrder
        };

        var created = await unitOfWork.Repository<TestParameter>().AddAsync(parameter, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TestParameterResponseDto>.Success(new TestParameterResponseDto
        {
            Id = created.Id,
            DiagnosticTestId = created.DiagnosticTestId,
            ParameterName = created.ParameterName,
            Unit = created.Unit,
            ReferenceRangeMin = created.ReferenceRangeMin,
            ReferenceRangeMax = created.ReferenceRangeMax,
            DataType = created.DataType,
            DisplayOrder = created.DisplayOrder,
            CreatedAt = created.CreatedAt
        });
    }
}
