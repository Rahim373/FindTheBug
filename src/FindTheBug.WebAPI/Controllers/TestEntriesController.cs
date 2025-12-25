using FindTheBug.Application.Features.Laboratory.TestEntries.Commands;
using FindTheBug.Application.Features.Laboratory.TestEntries.DTOs;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test entry management endpoints
/// </summary>
public class TestEntriesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Create a new test entry
    /// </summary>
    /// <param name="command">Test entry creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created test entry</returns>
    /// <response code="200">Returns the newly created test entry</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.Create)]
    [ProducesResponseType(typeof(TestEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateTestEntryCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            entry => Ok(entry),
            Problem);
    }
}