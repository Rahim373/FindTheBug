using FindTheBug.Application.Features.Invoices.Commands;
using FindTheBug.Application.Features.Invoices.Queries;
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

    /// <summary>
    /// Generate invoice PDF
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file</returns>
    [HttpGet("{id}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePdf(Guid id, CancellationToken cancellationToken)
    {
        var query = new GenerateInvoicePdfQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            pdfBytes => File(pdfBytes, "application/pdf", $"Invoice-{id}.pdf"),
            errors => NotFound(new { errors })
        );
    }
}

