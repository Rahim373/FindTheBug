using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IQuery<Patient>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetPatientByIdQuery, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(request.Id, cancellationToken);
        
        if (patient is null)
            return Error.NotFound("Patient.NotFound", $"Patient with ID {request.Id} not found");

        return patient;
    }
}
