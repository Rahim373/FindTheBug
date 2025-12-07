using FindTheBug.Application.Features.Modules.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModulesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllModulesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
