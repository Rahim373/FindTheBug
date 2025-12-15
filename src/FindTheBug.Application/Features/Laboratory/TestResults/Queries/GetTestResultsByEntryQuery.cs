using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestResults.DTOs;

namespace FindTheBug.Application.Features.Laboratory.TestResults.Queries;

public record GetTestResultsByEntryQuery(Guid TestEntryId) : IQuery<List<TestResultResponseDto>>;