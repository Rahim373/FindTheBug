using FindTheBug.Application.Features.TestEntries.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test entry management endpoints
/// </summary>
public class TestEntriesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Register patient for test
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestEntryCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            testEntry => Ok(testEntry),
            errors => Problem(errors));
    }
}
