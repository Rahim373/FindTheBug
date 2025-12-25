using FindTheBug.Application.Features.Billing.Invoices.Commands;
using FindTheBug.Application.Features.Billing.Invoices.DTOs;
using FindTheBug.Domain.Common;
using FindTheBug.WebAPI.Attributes;
using FindTheBug.Application.Features.Billing.Invoices.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FindTheBug.Domain.Contracts;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Invoice management endpoints
/// </summary>
public class InvoicesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Create a new invoice
    /// </summary>
    /// <param name="command">Invoice creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created invoice</returns>
    /// <response code="200">Returns the newly created invoice</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Billing, ModulePermission.Create)]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            invoice => Ok(invoice),
            Problem);
    }

    /// <summary>
    /// Generate PDF for an invoice
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file</returns>
    /// <response code="200">Returns the PDF file</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the invoice is not found</response>
    [HttpGet("{id}/pdf")]
    [RequireModulePermission(ModuleConstants.Billing, ModulePermission.View)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePdf(Guid id, CancellationToken cancellationToken)
    {
        var query = new GenerateInvoicePdfQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            pdf => File(pdf, "application/pdf", $"Invoice-{id}.pdf"),
            Problem);
    }
}