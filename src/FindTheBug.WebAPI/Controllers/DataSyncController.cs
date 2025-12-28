using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;
using FindTheBug.Application.Features.UserManagement.Modules.DTOs;
using FindTheBug.Application.Features.UserManagement.Users.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
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

    /// <summary>
    /// Get all diagnostic tests with optional search, category, and pagination
    /// </summary>
    /// <param name="search">Search by test name, test code, or category</param>
    /// <param name="category">Filter by category</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of diagnostic tests</returns>
    /// <response code="200">Returns paginated list of diagnostic tests</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet("diagnostic-tests")]
    [ProducesResponseType(typeof(PagedResult<DiagnosticTestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken,
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllDiagnosticTestsQuery(search, category, isActive, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            tests => Ok(tests),
            Problem);
    }

}
