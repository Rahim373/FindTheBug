using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestParameters.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Handlers;

public class GetAllTestParametersQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetAllTestParametersQuery, IEnumerable<TestParameter>>
{
    public async Task<ErrorOr<IEnumerable<TestParameter>>> Handle(GetAllTestParametersQuery request, CancellationToken cancellationToken)
    {
        var parameters = await unitOfWork.Repository<TestParameter>().GetAllAsync(cancellationToken);
        
        if (request.DiagnosticTestId.HasValue)
        {
            parameters = parameters.Where(p => p.DiagnosticTestId == request.DiagnosticTestId.Value);
        }

        return parameters.OrderBy(p => p.DisplayOrder).ToList();
    }
}
