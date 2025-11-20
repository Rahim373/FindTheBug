using FindTheBug.Application.Features.TestEntries.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test entry management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestEntriesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Register patient for test
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestEntryCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
