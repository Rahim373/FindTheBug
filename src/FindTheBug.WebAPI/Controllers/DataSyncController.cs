using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Application.Features.UserManagement.Modules.DTOs;
using FindTheBug.Application.Features.UserManagement.Users.Queries;
using FindTheBug.Domain.Entities;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[Route("api/service-sync")]
[BasicAuth]
public class DataSyncController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all modules
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all doctors</returns>
    /// <response code="200">Returns the list of doctors</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("doctors")]
    [ProducesResponseType(typeof(List<Doctor>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllModules(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDoctorsSyncQuery(pageNumber,pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            modules => Ok(modules),
            Problem);
    }

    /// <summary>
    /// Get all modules
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all modules</returns>
    /// <response code="200">Returns the list of modules</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<ModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersSyncQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            users => Ok(users),
            Problem);
    }

}
