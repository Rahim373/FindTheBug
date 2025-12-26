using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.TestResults.Commands;
using FindTheBug.Application.Features.Laboratory.TestResults.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.TestResults.Handlers;

public class CreateTestResultCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTestResultCommand, TestResultResponseDto>
{
    public async Task<ErrorOr<Result<TestResultResponseDto>>> Handle(CreateTestResultCommand request, CancellationToken cancellationToken)
    {
        var result = new TestResult
        {
            TestEntryId = request.TestEntryId,
            TestParameterId = request.TestParameterId,
            ResultValue = request.ResultValue,
            IsAbnormal = request.IsAbnormal,
            Notes = request.Notes
        };

        var created = await unitOfWork.Repository<TestResult>().AddAsync(result, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Load parameter name
        var parameter = await unitOfWork.Repository<TestParameter>()
            .GetByIdAsync(created.TestParameterId, cancellationToken);

        return Result<TestResultResponseDto>.Success(new TestResultResponseDto
        {
            Id = created.Id,
            TestEntryId = created.TestEntryId,
            TestParameterId = created.TestParameterId,
            ParameterName = parameter?.ParameterName ?? string.Empty,
            ResultValue = created.ResultValue,
            IsAbnormal = created.IsAbnormal,
            Notes = created.Notes,
            CreatedAt = created.CreatedAt
        });
    }
}
