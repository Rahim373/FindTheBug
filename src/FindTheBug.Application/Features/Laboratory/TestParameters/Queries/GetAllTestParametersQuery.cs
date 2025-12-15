using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Queries;

public record GetAllTestParametersQuery(Guid? DiagnosticTestId) : IQuery<List<TestParameterResponseDto>>;