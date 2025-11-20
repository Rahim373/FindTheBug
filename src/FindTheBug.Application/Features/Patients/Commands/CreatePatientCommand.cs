using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.Patients.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string MobileNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Email,
    string? Address
) : IRequest<Result<Patient>>;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreatePatientCommand, Result<Patient>>
{
    public async Task<Result<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        try
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
            return Result<Patient>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<Patient>.Failure($"Error creating patient: {ex.Message}");
        }
    }
}
