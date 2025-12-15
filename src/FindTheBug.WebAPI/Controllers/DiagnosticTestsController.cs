using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Diagnostic test management endpoints
/// </summary>
public class DiagnosticTestsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Create a new diagnostic test
    /// </summary>
    /// <param name="command">Diagnostic test creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created diagnostic test</returns>
    /// <response code="200">Returns the newly created diagnostic test</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(DiagnosticTestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDiagnosticTestCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            test => Ok(test),
            Problem);
    }
}
