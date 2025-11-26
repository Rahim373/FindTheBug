using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreatePatientCommand, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            TenantId = string.Empty, // Will be set by DbContext
            PatientCode = $"PAT-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNumber = request.MobileNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = request.Email,
            Address = request.Address,
            IsActive = true
        };

        var created = await unitOfWork.Repository<Patient>().AddAsync(patient, cancellationToken);
        return created;
    }
}
