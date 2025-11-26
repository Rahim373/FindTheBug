using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.DiagnosticTests.Commands;

public record CreateDiagnosticTestCommand(
    string TestCode,
    string TestName,
    string? Description,
    string Category,
    decimal Price,
    int? Duration,
    bool RequiresFasting
) : ICommand<DiagnosticTest>;
