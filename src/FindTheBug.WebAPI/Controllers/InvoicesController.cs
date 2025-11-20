using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Invoices.Commands;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Invoice and billing management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController(IMediator mediator, IUnitOfWork unitOfWork, ILogger<InvoicesController> logger) : ControllerBase
{
    /// <summary>
    /// Get all invoices with optional status filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<Invoice>>>> GetAll([FromQuery] string? status, CancellationToken cancellationToken)
    {
        try
        {
            var invoices = await unitOfWork.Repository<Invoice>().GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(status))
            {
                invoices = invoices.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(Result<IEnumerable<Invoice>>.Success(invoices.OrderByDescending(i => i.InvoiceDate)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving invoices");
            return StatusCode(500, Result<IEnumerable<Invoice>>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Get invoice by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<Invoice>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await unitOfWork.Repository<Invoice>().GetByIdAsync(id, cancellationToken);
            if (invoice is null)
                return NotFound(Result<Invoice>.Failure($"Invoice with ID {id} not found"));

            return Ok(Result<Invoice>.Success(invoice));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving invoice {Id}", id);
            return StatusCode(500, Result<Invoice>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Create invoice from test entries
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<Invoice>>> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, Result<Invoice>.Failure("An error occurred"));
        }
    }
}
