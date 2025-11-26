using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Handlers;

public class VerifyTestResultsCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<VerifyTestResultsCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(VerifyTestResultsCommand request, CancellationToken cancellationToken)
    {
        var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
        var entryResults = results.Where(r => r.TestEntryId == request.TestEntryId).ToList();

        if (!entryResults.Any())
            return Error.NotFound("TestResults.NotFound", $"No test results found for test entry {request.TestEntryId}");

        foreach (var result in entryResults)
        {
            result.VerifiedBy = request.VerifiedBy;
            result.VerifiedDate = DateTime.UtcNow;
            await unitOfWork.Repository<TestResult>().UpdateAsync(result, cancellationToken);
        }

        return true;
    }
}
