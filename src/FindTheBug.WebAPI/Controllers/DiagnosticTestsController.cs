using FindTheBug.Application.Features.DiagnosticTests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Diagnostic test management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiagnosticTestsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create new diagnostic test
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDiagnosticTestCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
