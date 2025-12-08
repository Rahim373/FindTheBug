using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.DTOs;

namespace FindTheBug.Application.Features.Patients.Commands;

public record CreatePatientCommand(
    string Name,
    string MobileNumber,
    int? Age,
    string? Gender,
    string? Address
) : ICommand<PatientResponseDto>;
