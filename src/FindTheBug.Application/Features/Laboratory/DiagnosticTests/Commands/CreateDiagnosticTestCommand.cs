using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;

public record CreateDiagnosticTestCommand(
    string TestName,
    string? TestCode,
    decimal Price,
    string? Description
) : ICommand<DiagnosticTestResponseDto>;
