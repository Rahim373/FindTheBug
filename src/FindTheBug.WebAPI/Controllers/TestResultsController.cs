using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Application.Features.TestResults.DTOs;
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
    /// <param name="entryId">Test entry ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of test results for the entry</returns>
    /// <response code="200">Returns the list of test results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("entry/{entryId}")]
    [ProducesResponseType(typeof(List<TestResultResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByEntry(Guid entryId, CancellationToken cancellationToken)
    {
        var query = new GetTestResultsByEntryQuery(entryId);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            testResults => Ok(testResults),
            Problem);
    }

    /// <summary>
    /// Create a test result
    /// </summary>
    /// <param name="command">Test result creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created test result</returns>
    /// <response code="200">Returns the newly created test result</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(TestResultResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTestResultCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            testResult => Ok(testResult),
            Problem);
    }

    /// <summary>
    /// Update a test result
    /// </summary>
    /// <param name="id">Test result ID</param>
    /// <param name="request">Test result update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated test result</returns>
    /// <response code="200">Returns the updated test result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the test result is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TestResultResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
    /// Verify test results for a test entry
    /// </summary>
    /// <param name="testEntryId">Test entry ID</param>
    /// <param name="request">Verification details including verifier name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success indicator</returns>
    /// <response code="200">Test results verified successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the test entry is not found</response>
    [HttpPost("{testEntryId}/verify")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verify(Guid testEntryId, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyTestResultsCommand(testEntryId, request.VerifiedBy);

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            success => Ok(success),
            Problem);
    }
}
