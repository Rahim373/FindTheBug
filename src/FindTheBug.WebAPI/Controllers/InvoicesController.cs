using FindTheBug.Application.Features.Invoices.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Invoice management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Create invoice from test entries
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
