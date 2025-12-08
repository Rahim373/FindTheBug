namespace FindTheBug.Application.Features.TestParameters.DTOs;

public record TestParameterResponseDto
{
    public Guid Id { get; init; }
    public Guid DiagnosticTestId { get; init; }
    public string ParameterName { get; init; } = string.Empty;
    public string? Unit { get; init; }
    public decimal? ReferenceRangeMin { get; init; }
    public decimal? ReferenceRangeMax { get; init; }
    public string DataType { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
}
