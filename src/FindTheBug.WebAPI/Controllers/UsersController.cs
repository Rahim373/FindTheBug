using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Application.Features.Users.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
public class UsersController(ISender mediator, IMapper mapper) : BaseApiController
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
        
        return result.Match(
            users => Ok(users),
            errors => Problem(errors));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            user => Ok(user),
            errors => Problem(errors));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateUserCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            user => Ok(user),
            errors => Problem(errors));
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateUserCommand>(request);
        // Ensure ID from route is used
        command = command with { Id = id };
        
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            user => Ok(user),
            errors => Problem(errors));
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}
