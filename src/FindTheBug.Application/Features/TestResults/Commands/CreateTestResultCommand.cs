using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Commands;

public record CreateTestResultCommand(
    Guid TestEntryId,
    Guid TestParameterId,
    string ResultValue,
    bool IsAbnormal,
    string? Notes
) : ICommand<TestResult>;