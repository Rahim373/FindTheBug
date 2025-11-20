using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetAllPatientsQuery(string? Search) : IRequest<Result<IEnumerable<Patient>>>;

public class GetAllPatientsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllPatientsQuery, Result<IEnumerable<Patient>>>
{
    public async Task<Result<IEnumerable<Patient>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patients = await unitOfWork.Repository<Patient>().GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(request.Search))
            {
                patients = patients.Where(p => 
                    p.FirstName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                    p.LastName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                    p.MobileNumber.Contains(request.Search));
            }

            return Result<IEnumerable<Patient>>.Success(patients);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Patient>>.Failure($"Error retrieving patients: {ex.Message}");
        }
    }
}
