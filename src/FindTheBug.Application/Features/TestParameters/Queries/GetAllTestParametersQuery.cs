using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestParameters.DTOs;

namespace FindTheBug.Application.Features.TestParameters.Queries;

public record GetAllTestParametersQuery(Guid? DiagnosticTestId) : IQuery<List<TestParameterResponseDto>>;