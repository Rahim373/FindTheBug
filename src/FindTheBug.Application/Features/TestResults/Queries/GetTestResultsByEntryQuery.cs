using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.DTOs;

namespace FindTheBug.Application.Features.TestResults.Queries;

public record GetTestResultsByEntryQuery(Guid TestEntryId) : IQuery<List<TestResultResponseDto>>;