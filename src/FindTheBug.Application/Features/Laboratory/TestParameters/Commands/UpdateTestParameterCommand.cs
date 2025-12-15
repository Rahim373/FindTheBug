using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Commands;

public record UpdateTestParameterCommand(
    Guid Id,
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
) : ICommand<TestParameterResponseDto>;