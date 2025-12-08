namespace FindTheBug.Application.Features.Patients.DTOs;

public record PatientResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public int? Age { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record PatientListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public int? Age { get; init; }
    public string? Gender { get; init; }
}
