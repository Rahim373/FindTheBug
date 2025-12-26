using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.TestResults.Commands;
using FindTheBug.Application.Features.Laboratory.TestResults.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.TestResults.Handlers;

public class UpdateTestResultCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTestResultCommand, TestResultResponseDto>
{
    public async Task<ErrorOr<Result<TestResultResponseDto>>> Handle(UpdateTestResultCommand request, CancellationToken cancellationToken)
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

        return Result<TestResultResponseDto>.Success(new TestResultResponseDto
        {
            Id = result.Id,
            TestEntryId = result.TestEntryId,
            TestParameterId = result.TestParameterId,
            ParameterName = result.TestParameter?.ParameterName ?? string.Empty,
            ResultValue = result.ResultValue,
            IsAbnormal = result.IsAbnormal,
            Notes = result.Notes,
            CreatedAt = result.CreatedAt
        });
    }
}
