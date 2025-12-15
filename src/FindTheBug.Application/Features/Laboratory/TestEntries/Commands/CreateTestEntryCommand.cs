using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestEntries.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestEntries.Commands;

public record CreateTestEntryCommand(
    Guid PatientId,
    Guid DiagnosticTestId,
    DateTime EntryDate
) : ICommand<TestEntryResponseDto>;
