using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestResults.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestResults.Commands;

public record CreateTestResultCommand(
    Guid TestEntryId,
    Guid TestParameterId,
    string ResultValue,
    bool IsAbnormal,
    string? Notes
) : ICommand<TestResultResponseDto>;