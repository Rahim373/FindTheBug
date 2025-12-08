using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Application.Features.TestResults.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test result management endpoints
/// </summary>
public class TestResultsController(ISender mediator, IMapper mapper) : BaseApiController
{
    /// <summary>
    /// Get test results for a test entry
    /// </summary>
    [HttpGet("entry/{entryId}")]
    public async Task<IActionResult> GetByEntry(Guid entryId, CancellationToken cancellationToken)
    {
        var query = new GetTestResultsByEntryQuery(entryId);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            testResults => Ok(testResults),
            Problem);
    }

    /// <summary>
    /// Create test result
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestResultCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            testResult => Ok(testResult),
            Problem);
    }

    /// <summary>
    /// Update test result
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestResultRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateTestResultCommand>(request);
        command = command with { Id = id };

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            testResult => Ok(testResult),
            Problem);
    }

    /// <summary>
    /// Verify test results
    /// </summary>
    [HttpPost("{testEntryId}/verify")]
    public async Task<IActionResult> Verify(Guid testEntryId, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyTestResultsCommand( testEntryId, request.VerifiedBy);

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            success => Ok(success),
            Problem);
    }
}
