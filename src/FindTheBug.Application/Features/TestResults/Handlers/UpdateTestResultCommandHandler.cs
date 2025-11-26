using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Handlers;

public class UpdateTestResultCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<UpdateTestResultCommand, TestResult>
{
    public async Task<ErrorOr<TestResult>> Handle(UpdateTestResultCommand request, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<TestResult>().GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            return Error.NotFound("TestResult.NotFound", $"Test result with ID {request.Id} not found");

        existing.ResultValue = request.ResultValue;
        existing.IsAbnormal = request.IsAbnormal;
        existing.Notes = request.Notes;

        await unitOfWork.Repository<TestResult>().UpdateAsync(existing, cancellationToken);
        return existing;
    }
}
