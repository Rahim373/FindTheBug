namespace FindTheBug.Application.Features.Doctors.DTOs;

public record DoctorResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Degree { get; init; }
    public string? Office { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public List<DoctorSpecialityDto> Specialities { get; init; } = new();
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record DoctorListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Degree { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public List<string> SpecialityNames { get; init; } = new();
    public bool IsActive { get; init; }
}

public record DoctorSpecialityDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public record CreateDoctorCommandDto
{
    public required string Name { get; init; }
    public string? Degree { get; init; }
    public string? Office { get; init; }
    public required string PhoneNumber { get; init; }
    public List<Guid> SpecialityIds { get; init; } = new();
}

public record UpdateDoctorCommandDto
{
    public required string Name { get; init; }
    public string? Degree { get; init; }
    public string? Office { get; init; }
    public required string PhoneNumber { get; init; }
    public List<Guid> SpecialityIds { get; init; } = new();
    public bool IsActive { get; init; }
}
