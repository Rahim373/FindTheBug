using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Queries;

public class GetTestResultsByEntryQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetTestResultsByEntryQuery, IEnumerable<TestResult>>
{
    public async Task<ErrorOr<IEnumerable<TestResult>>> Handle(GetTestResultsByEntryQuery request, CancellationToken cancellationToken)
    {
        var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
        return results.Where(r => r.TestEntryId == request.TestEntryId).ToList();
    }
}
