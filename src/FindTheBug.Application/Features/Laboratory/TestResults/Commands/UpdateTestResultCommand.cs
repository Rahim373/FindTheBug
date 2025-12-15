using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestResults.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestResults.Commands;

public record UpdateTestResultCommand(
    Guid Id,
    string ResultValue,
    bool IsAbnormal,
    string? Notes
) : ICommand<TestResultResponseDto>;