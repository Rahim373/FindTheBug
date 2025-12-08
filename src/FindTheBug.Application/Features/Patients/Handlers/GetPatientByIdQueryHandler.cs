using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Application.Features.Patients.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetPatientByIdQuery, PatientResponseDto>
{
    public async Task<ErrorOr<PatientResponseDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(request.Id, cancellationToken);

        if (patient == null)
            return Error.NotFound("Patient.NotFound", "Patient not found");

        return new PatientResponseDto
        {
            Id = patient.Id,
            Name = patient.FirstName,
            MobileNumber = patient.MobileNumber,
            Age = patient.Age,
            Gender = patient.Gender,
            Address = patient.Address,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        };
    }
}
