using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.DTOs;

namespace FindTheBug.Application.Features.TestResults.Commands;

public record UpdateTestResultCommand(
    Guid Id,
    string ResultValue,
    bool IsAbnormal,
    string? Notes
) : ICommand<TestResultResponseDto>;