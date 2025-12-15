using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;

namespace FindTheBug.Application.Features.Doctors.Commands;

public record UpdateDoctorCommand(
    Guid Id,
    string Name,
    string? Degree,
    string? Office,
    string PhoneNumber,
    List<Guid> SpecialityIds,
    bool IsActive
) : ICommand<DoctorResponseDto>;
