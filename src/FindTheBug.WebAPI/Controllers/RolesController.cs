using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Roles.Commands;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;
using FindTheBug.Application.Features.UserManagement.Roles.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using FindTheBug.WebAPI.Contracts.Requests;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Role management endpoints
/// </summary>
public class RolesController(ISender mediator, IMapper mapper) : BaseApiController
{
    /// <summary>
    /// Get all roles with pagination and search
    /// </summary>
    /// <param name="search">Optional search term to filter roles by name or description</param>
    /// <param name="pageNumber">Page number (default:1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of roles</returns>
    /// <response code="200">Returns paginated list of roles</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.View)]
    [ProducesResponseType(typeof(PagedResult<RoleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllRolesQuery(pageNumber, pageSize, search);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            roles => Ok(roles),
            Problem);
    }

    /// <summary>
    /// Get all active roles
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active roles</returns>
    /// <response code="200">Returns list of active roles</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet("active")]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.View)]
    [ProducesResponseType(typeof(List<RoleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var query = new GetActiveRolesQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            roles => Ok(roles),
            Problem);
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role details including module permissions</returns>
    /// <response code="200">Returns role</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If role is not found</response>
    [HttpGet("{id:guid}")]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.View)]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            role => Ok(role),
            Problem);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="request">Role creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created role</returns>
    /// <response code="200">Returns newly created role</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="409">If a role with same name already exists</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.Create)]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = mapper.Map<CreateRoleCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            role => Ok(role),
            Problem);
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="request">Role update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated role</returns>
    /// <response code="200">Returns updated role</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If role is not found</response>
    /// <response code="409">If a role with same name already exists</response>
    [HttpPut("{id:guid}")]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.Edit)]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = mapper.Map<UpdateRoleCommand>(request);
        command = command with { Id = id };

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            role => Ok(role),
            Problem);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Role successfully deleted</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If role is not found</response>
    /// <response code="400">If role cannot be deleted (e.g., system role)</response>
    [HttpDelete("{id:guid}")]
    [RequireModulePermission(ModuleConstants.UserManagement, ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}