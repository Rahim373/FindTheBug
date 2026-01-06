using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Reception.Commands;
using FindTheBug.Application.Features.Reception.DTOs;
using FindTheBug.Application.Features.Reception.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Receipt management endpoints
/// </summary>
[Authorize]
public class ReceiptsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all receipts with optional search and pagination
    /// </summary>
    /// <param name="search">Search by invoice number, name, phone number, or address</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of receipts</returns>
    /// <response code="200">Returns paginated list of receipts</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Reception, ModulePermission.View)]
    [ProducesResponseType(typeof(PagedResult<ReceiptListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllReceiptsQuery(search, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            receipts => Ok(receipts),
            Problem);
    }

    /// <summary>
    /// Get receipt by ID
    /// </summary>
    /// <param name="id">Receipt ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Receipt details</returns>
    /// <response code="200">Returns receipt</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If receipt is not found</response>
    [HttpGet("{id}")]
    [RequireModulePermission(ModuleConstants.Reception, ModulePermission.View)]
    [ProducesResponseType(typeof(ReceiptResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReceiptByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            receipt => Ok(receipt),
            Problem);
    }

    /// <summary>
    /// Create new receipt
    /// </summary>
    /// <param name="command">Receipt creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created receipt</returns>
    /// <response code="200">Returns newly created receipt</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="409">If a receipt with the same invoice number already exists</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Reception, ModulePermission.Create)]
    [ProducesResponseType(typeof(ReceiptResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReceiptCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            receipt => Ok(receipt),
            Problem);
    }

    /// <summary>
    /// Update existing receipt
    /// </summary>
    /// <param name="id">Receipt ID</param>
    /// <param name="command">Receipt update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated receipt</returns>
    /// <response code="200">Returns updated receipt</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If receipt is not found</response>
    /// <response code="409">If another receipt with the same invoice number already exists</response>
    [HttpPut("{id}")]
    [RequireModulePermission(ModuleConstants.Reception, ModulePermission.Edit)]
    [ProducesResponseType(typeof(ReceiptResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReceiptCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await mediator.Send(updateCommand, cancellationToken);

        return result.Match(
            receipt => Ok(receipt),
            Problem);
    }

    /// <summary>
    /// Delete receipt
    /// </summary>
    /// <param name="id">Receipt ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Receipt deleted successfully</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If receipt is not found</response>
    [HttpDelete("{id}")]
    [RequireModulePermission(ModuleConstants.Reception, ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteReceiptCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}