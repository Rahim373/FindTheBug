using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IRequest<Result<Patient>>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPatientByIdQuery, Result<Patient>>
{
    public async Task<Result<Patient>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(request.Id, cancellationToken);
            
            if (patient is null)
                return Result<Patient>.Failure($"Patient with ID {request.Id} not found");

            return Result<Patient>.Success(patient);
        }
        catch (Exception ex)
        {
            return Result<Patient>.Failure($"Error retrieving patient: {ex.Message}");
        }
    }
}
