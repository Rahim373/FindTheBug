using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.Application.Features.Roles.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllRolesQuery(pageNumber, pageSize, search);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var query = new GetActiveRolesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateRoleCommand(
            request.Name,
            request.Description,
            request.IsActive
        );

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateRoleCommand(
            id,
            request.Name,
            request.Description,
            request.IsActive
        );

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
