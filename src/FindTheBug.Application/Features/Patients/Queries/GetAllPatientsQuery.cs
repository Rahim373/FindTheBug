using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetAllPatientsQuery(string? Search) : IQuery<IEnumerable<Patient>>;

public class GetAllPatientsQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetAllPatientsQuery, IEnumerable<Patient>>
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
