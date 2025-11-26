using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestEntries.Commands;

public record CreateTestEntryCommand(
    Guid PatientId,
    Guid DiagnosticTestId,
    DateTime? SampleCollectionDate,
    string Priority,
    string? ReferredBy,
    string? Notes
) : ICommand<TestEntry>;
