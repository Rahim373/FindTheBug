using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Application.Features.Patients.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetPatientByIdQuery, PatientResponseDto>
{
    public async Task<ErrorOr<Result<PatientResponseDto>>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Repository<LabReceipt>().GetByIdAsync(request.Id, cancellationToken);

        if (patient == null)
            return Error.NotFound("Patient.NotFound", "Patient not found");

        return Result<PatientResponseDto>.Success(new PatientResponseDto
        {
            Id = patient.Id,
            Name = patient.FullName,
            MobileNumber = patient.PhoneNumber,
            Age = patient.Age,
            Gender = patient.Gender,
            Address = patient.Address,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        });
    }
}
