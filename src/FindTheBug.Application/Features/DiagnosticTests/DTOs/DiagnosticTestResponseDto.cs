namespace FindTheBug.Application.Features.DiagnosticTests.DTOs;

public record DiagnosticTestResponseDto
{
    public Guid Id { get; init; }
    public string TestName { get; init; } = string.Empty;
    public string? TestCode { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
