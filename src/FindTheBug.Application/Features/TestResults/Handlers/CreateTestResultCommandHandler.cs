using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Handlers;

public class CreateTestResultCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateTestResultCommand, TestResult>
{
    public async Task<ErrorOr<TestResult>> Handle(CreateTestResultCommand request, CancellationToken cancellationToken)
    {
        var testResult = new TestResult
        {
            Id = Guid.NewGuid(),
            TenantId = string.Empty,
            TestEntryId = request.TestEntryId,
            TestParameterId = request.TestParameterId,
            ResultValue = request.ResultValue,
            IsAbnormal = request.IsAbnormal,
            Notes = request.Notes,
            ResultDate = DateTime.UtcNow
        };

        var created = await unitOfWork.Repository<TestResult>().AddAsync(testResult, cancellationToken);
        return created;
    }
}
