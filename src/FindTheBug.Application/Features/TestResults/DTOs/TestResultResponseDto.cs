namespace FindTheBug.Application.Features.TestResults.DTOs;

public record TestResultResponseDto
{
    public Guid Id { get; init; }
    public Guid TestEntryId { get; init; }
    public Guid TestParameterId { get; init; }
    public string ParameterName { get; init; } = string.Empty;
    public string ResultValue { get; init; } = string.Empty;
    public bool IsAbnormal { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
