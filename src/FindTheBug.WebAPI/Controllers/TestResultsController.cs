using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Application.Features.TestResults.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test result management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestResultsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get test results for a test entry
    /// </summary>
    [HttpGet("entry/{entryId}")]
    public async Task<IActionResult> GetByEntry(Guid entryId, CancellationToken cancellationToken)
    {
        var query = new GetTestResultsByEntryQuery(entryId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create test result
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestResultCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update test result
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestResultRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTestResultCommand(id, request.ResultValue, request.IsAbnormal, request.Notes);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Verify test results
    /// </summary>
    [HttpPost("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyTestResultsCommand(id, request.VerifiedBy);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
