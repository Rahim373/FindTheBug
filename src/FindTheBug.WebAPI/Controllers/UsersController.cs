using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Application.Features.Users.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get all users with optional search and pagination
    /// </summary>
    /// <param name="search">Search by name, email, phone, or NID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery(search, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.NIDNumber,
            request.RoleIds,
            request.IsActive,
            request.AllowUserLogin
        );

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            id,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.NIDNumber,
            request.RoleIds,
            request.IsActive,
            request.AllowUserLogin,
            request.Password
        );

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
