using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetAllPatientsQuery(string? Search) : IRequest<ErrorOr<IEnumerable<Patient>>>;

public class GetAllPatientsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllPatientsQuery, ErrorOr<IEnumerable<Patient>>>
{
    public async Task<ErrorOr<IEnumerable<Patient>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await unitOfWork.Repository<Patient>().GetAllAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            patients = patients.Where(p => 
                p.FirstName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                p.LastName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                p.MobileNumber.Contains(request.Search));
        }

        return patients.ToList();
    }
}
