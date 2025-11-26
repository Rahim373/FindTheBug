using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string MobileNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Email,
    string? Address
) : ICommand<Patient>;
