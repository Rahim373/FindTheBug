using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePatientCommand, PatientResponseDto>
{
    public async Task<ErrorOr<Result<PatientResponseDto>>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Check if patient with mobile number exists
        var existing = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .FirstOrDefaultAsync(p => p.PhoneNumber == request.MobileNumber, cancellationToken);

        if (existing != null)
            return Error.Conflict("Patient.MobileExists", "Patient with this mobile number already exists");

        var patient = new LabReceipt
        {
            FullName = request.Name,
            PhoneNumber = request.MobileNumber,
            Age = request.Age,
            Gender = request.Gender,
            Address = request.Address
        };

        var created = await unitOfWork.Repository<LabReceipt>().AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PatientResponseDto>.Success(new PatientResponseDto
        {
            Id = created.Id,
            Name = created.FullName,
            MobileNumber = created.PhoneNumber,
            Age = created.Age,
            Gender = created.Gender,
            Address = created.Address,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        });
    }
}
