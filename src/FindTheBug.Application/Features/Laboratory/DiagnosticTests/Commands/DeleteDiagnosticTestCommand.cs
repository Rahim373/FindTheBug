using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;

public record DeleteDiagnosticTestCommand(Guid Id) : ICommand<bool>;