using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestParameters.DTOs;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public record UpdateTestParameterCommand(
    Guid Id,
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
) : ICommand<TestParameterResponseDto>;