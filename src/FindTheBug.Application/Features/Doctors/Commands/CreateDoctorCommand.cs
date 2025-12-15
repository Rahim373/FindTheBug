using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;

namespace FindTheBug.Application.Features.Doctors.Commands;

public record CreateDoctorCommand(
    string Name,
    string? Degree,
    string? Office,
    string PhoneNumber,
    List<Guid> SpecialityIds
) : ICommand<DoctorResponseDto>;
