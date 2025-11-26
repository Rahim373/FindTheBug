using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Queries;

public record GetAllTestParametersQuery(Guid? DiagnosticTestId = null) 
    : IQuery<IEnumerable<TestParameter>>;