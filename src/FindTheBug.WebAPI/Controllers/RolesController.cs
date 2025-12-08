using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.Application.Features.Roles.Queries;
using FindTheBug.WebAPI.Contracts.Requests;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

public class RolesController(ISender mediator, IMapper mapper) : BaseApiController
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
        
        return result.Match(
            roles => Ok(roles),
            errors => Problem(errors));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var query = new GetActiveRolesQuery();
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            roles => Ok(roles),
            errors => Problem(errors));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            role => Ok(role),
            errors => Problem(errors));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = mapper.Map<CreateRoleCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            role => Ok(role),
            errors => Problem(errors));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = mapper.Map<UpdateRoleCommand>(request);
        // Ensure ID from route is used
        command = command with { Id = id };
        
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            role => Ok(role),
            errors => Problem(errors));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}
