using FindTheBug.Application.Features.Modules.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

public class ModulesController(ISender mediator) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllModulesQuery();
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            modules => Ok(modules),
            errors => Problem(errors));
    }
}
