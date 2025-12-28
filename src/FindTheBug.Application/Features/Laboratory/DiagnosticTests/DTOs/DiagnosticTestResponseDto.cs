namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;

public record DiagnosticTestResponseDto
{
    public Guid Id { get; init; }
    public string TestName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string? Duration { get; init; }
    public bool RequiresFasting { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}