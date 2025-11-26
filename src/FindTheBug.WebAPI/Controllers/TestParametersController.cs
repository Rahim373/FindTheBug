using FindTheBug.Application.Features.TestParameters.Commands;
using FindTheBug.Application.Features.TestParameters.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test parameter management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestParametersController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get all test parameters for a diagnostic test
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? diagnosticTestId, CancellationToken cancellationToken)
    {
        var query = new GetAllTestParametersQuery(diagnosticTestId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create new test parameter
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestParameterCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update test parameter
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestParameterRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTestParameterCommand(
            id,
            request.ParameterName,
            request.Unit,
            request.ReferenceRangeMin,
            request.ReferenceRangeMax,
            request.DataType,
            request.DisplayOrder
        );
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete test parameter
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTestParameterCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
