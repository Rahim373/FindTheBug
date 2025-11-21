using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.Application.Features.TestResults.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test results management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestResultsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Record test results for a test entry
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestResultCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all results for a test entry
    /// </summary>
    [HttpGet("entry/{testEntryId}")]
    public async Task<IActionResult> GetByTestEntry(Guid testEntryId, CancellationToken cancellationToken)
    {
        var query = new GetTestResultsByEntryQuery(testEntryId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update test result
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestResultRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTestResultCommand(
            id,
            request.ResultValue,
            request.IsAbnormal,
            request.Notes
        );
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Verify test results
    /// </summary>
    [HttpPost("{testEntryId}/verify")]
    public async Task<IActionResult> VerifyResults(Guid testEntryId, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyTestResultsCommand(testEntryId, request.VerifiedBy);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

public record UpdateTestResultRequest(
    string ResultValue,
    bool IsAbnormal,
    string? Notes
);

public record VerifyRequest(string VerifiedBy);
