using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Application.Features.TestResults.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.TestResults.Handlers;

public class UpdateTestResultCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTestResultCommand, TestResultResponseDto>
{
    public async Task<ErrorOr<TestResultResponseDto>> Handle(UpdateTestResultCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Repository<TestResult>().GetQueryable()
            .Include(tr => tr.TestParameter)
            .FirstOrDefaultAsync(tr => tr.Id == request.Id, cancellationToken);

        if (result == null)
            return Error.NotFound("TestResult.NotFound", "Test result not found");

        result.ResultValue = request.ResultValue;
        result.IsAbnormal = request.IsAbnormal;
        result.Notes = request.Notes;

        await unitOfWork.Repository<TestResult>().UpdateAsync(result);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TestResultResponseDto
        {
            Id = result.Id,
            TestEntryId = result.TestEntryId,
            TestParameterId = result.TestParameterId,
            ParameterName = result.TestParameter?.ParameterName ?? string.Empty,
            ResultValue = result.ResultValue,
            IsAbnormal = result.IsAbnormal,
            Notes = result.Notes,
            CreatedAt = result.CreatedAt
        };
    }
}
