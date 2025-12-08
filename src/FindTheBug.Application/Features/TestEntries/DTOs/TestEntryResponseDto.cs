namespace FindTheBug.Application.Features.TestEntries.DTOs;

public record TestEntryResponseDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid DiagnosticTestId { get; init; }
    public string TestName { get; init; } = string.Empty;
    public DateTime EntryDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
