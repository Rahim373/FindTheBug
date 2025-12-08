using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePatientCommand, PatientResponseDto>
{
    public async Task<ErrorOr<PatientResponseDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Check if patient with mobile number exists
        var existing = await unitOfWork.Repository<Patient>().GetQueryable()
            .FirstOrDefaultAsync(p => p.MobileNumber == request.MobileNumber, cancellationToken);

        if (existing != null)
            return Error.Conflict("Patient.MobileExists", "Patient with this mobile number already exists");

        var patient = new Patient
        {
            Name = request.Name,
            MobileNumber = request.MobileNumber,
            Age = request.Age,
            Gender = request.Gender,
            Address = request.Address
        };

        var created = await unitOfWork.Repository<Patient>().AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PatientResponseDto
        {
            Id = created.Id,
            Name = created.Name,
            MobileNumber = created.MobileNumber,
            Age = created.Age,
            Gender = created.Gender,
            Address = created.Address,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
