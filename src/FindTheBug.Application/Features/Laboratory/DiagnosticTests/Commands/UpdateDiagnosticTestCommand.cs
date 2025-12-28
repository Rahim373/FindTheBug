using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;

public record UpdateDiagnosticTestCommand(
    Guid Id,
    string TestName,
    string? TestCode,
    string Category,
    decimal Price,
    string? Description,
    string? Duration,
    bool RequiresFasting,
    bool IsActive
) : ICommand<DiagnosticTestResponseDto>;