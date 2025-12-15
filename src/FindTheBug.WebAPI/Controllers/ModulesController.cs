using FindTheBug.Application.Features.UserManagement.Modules.DTOs;
using FindTheBug.Application.Features.UserManagement.Modules.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Module management endpoints
/// </summary>
public class ModulesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all modules
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all modules</returns>
    /// <response code="200">Returns the list of modules</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllModulesQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            modules => Ok(modules),
            Problem);
    }
}
