using FindTheBug.Application.Features.Laboratory.TestParameters.Commands;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;
using FindTheBug.Domain.Common;
using FindTheBug.WebAPI.Attributes;
using FindTheBug.Application.Features.Laboratory.TestParameters.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test parameter management endpoints
/// </summary>
public class TestParametersController(ISender mediator, IMapper mapper) : BaseApiController
{
    /// <summary>
    /// Get all test parameters with optional filter by diagnostic test
    /// </summary>
    /// <param name="diagnosticTestId">Optional diagnostic test ID to filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of test parameters</returns>
    /// <response code="200">Returns the list of test parameters</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission("Laboratory", ModulePermission.View)]
    [ProducesResponseType(typeof(List<TestParameterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? diagnosticTestId, CancellationToken cancellationToken)
    {
        var query = new GetAllTestParametersQuery(diagnosticTestId);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            parameters => Ok(parameters),
            Problem);
    }

    /// <summary>
    /// Create a new test parameter
    /// </summary>
    /// <param name="command">Test parameter creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created test parameter</returns>
    /// <response code="200">Returns the newly created test parameter</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost]
    [RequireModulePermission("Laboratory", ModulePermission.Create)]
    [ProducesResponseType(typeof(TestParameterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateTestParameterCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            parameter => Ok(parameter),
            Problem);
    }

    /// <summary>
    /// Update an existing test parameter
    /// </summary>
    /// <param name="id">Test parameter ID</param>
    /// <param name="request">Test parameter update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated test parameter</returns>
    /// <response code="200">Returns the updated test parameter</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the test parameter is not found</response>
    [HttpPut("{id}")]
    [RequireModulePermission("Laboratory", ModulePermission.Edit)]
    [ProducesResponseType(typeof(TestParameterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestParameterCommand request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateTestParameterCommand>(request);
        command = command with { Id = id };

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            parameter => Ok(parameter),
            Problem);
    }

    /// <summary>
    /// Delete a test parameter
    /// </summary>
    /// <param name="id">Test parameter ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Test parameter successfully deleted</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the test parameter is not found</response>
    /// <response code="400">If the test parameter cannot be deleted</response>
    [HttpDelete("{id}")]
    [RequireModulePermission("Laboratory", ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTestParameterCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}