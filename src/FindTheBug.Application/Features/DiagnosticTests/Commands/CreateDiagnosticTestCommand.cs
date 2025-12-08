using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.DiagnosticTests.DTOs;

namespace FindTheBug.Application.Features.DiagnosticTests.Commands;

public record CreateDiagnosticTestCommand(
    string TestName,
    string? TestCode,
    decimal Price,
    string? Description
) : ICommand<DiagnosticTestResponseDto>;
