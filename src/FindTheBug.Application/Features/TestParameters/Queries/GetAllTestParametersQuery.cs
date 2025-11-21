using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.TestParameters.Queries;

public record GetAllTestParametersQuery(Guid? DiagnosticTestId = null) 
    : IRequest<ErrorOr<IEnumerable<TestParameter>>>;

public class GetAllTestParametersQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetAllTestParametersQuery, ErrorOr<IEnumerable<TestParameter>>>
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