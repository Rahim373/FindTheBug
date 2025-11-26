using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public record CreateTestParameterCommand(
    Guid DiagnosticTestId,
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
) : ICommand<TestParameter>;