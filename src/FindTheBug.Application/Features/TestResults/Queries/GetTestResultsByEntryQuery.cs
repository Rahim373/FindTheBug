using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestResults.Queries;

public record GetTestResultsByEntryQuery(Guid TestEntryId) : IQuery<IEnumerable<TestResult>>;