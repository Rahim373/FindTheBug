using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestEntries.DTOs;

namespace FindTheBug.Application.Features.TestEntries.Commands;

public record CreateTestEntryCommand(
    Guid PatientId,
    Guid DiagnosticTestId,
    DateTime EntryDate
) : ICommand<TestEntryResponseDto>;
