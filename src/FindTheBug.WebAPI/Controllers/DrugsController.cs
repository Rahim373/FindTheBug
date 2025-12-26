using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Drugs.Commands;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Application.Features.Dispensary.Drugs.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Drug management endpoints
/// </summary>
public class DrugsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all drugs with optional search and pagination
    /// </summary>
    /// <param name="search">Search by name, generic name, or brand</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of drugs</returns>
    /// <response code="200">Returns paginated list of drugs</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.View)]
    [ProducesResponseType(typeof(PagedResult<DrugListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllDrugsQuery(search, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            drugs => Ok(drugs),
            Problem);
    }

    /// <summary>
    /// Get drug by ID
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Drug details</returns>
    /// <response code="200">Returns drug</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If drug is not found</response>
    [HttpGet("{id}")]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.View)]
    [ProducesResponseType(typeof(DrugResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDrugByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            drug => Ok(drug),
            Problem);
    }

    /// <summary>
    /// Create new drug
    /// </summary>
    /// <param name="command">Drug creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created drug</returns>
    /// <response code="200">Returns newly created drug</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If generic name or brand not found</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.Create)]
    [ProducesResponseType(typeof(DrugResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateDrugCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            drug => Ok(drug),
            Problem);
    }

    /// <summary>
    /// Update existing drug
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <param name="command">Drug update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated drug</returns>
    /// <response code="200">Returns updated drug</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If drug, generic name, or brand is not found</response>
    [HttpPut("{id}")]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.Edit)]
    [ProducesResponseType(typeof(DrugResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDrugCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await mediator.Send(updateCommand, cancellationToken);

        return result.Match(
            drug => Ok(drug),
            Problem);
    }

    /// <summary>
    /// Delete drug
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Drug deleted successfully</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If drug is not found</response>
    [HttpDelete("{id}")]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDrugCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    /// <summary>
    /// Get all brands for dropdown
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of brands</returns>
    /// <response code="200">Returns list of brands</response>
    [HttpGet("brands")]
    [ProducesResponseType(typeof(List<BrandDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands(CancellationToken cancellationToken)
    {
        var query = new GetAllBrandsQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            brands => Ok(brands),
            Problem);
    }

    /// <summary>
    /// Get all generic names for dropdown with optional search
    /// </summary>
    /// <param name="search">Search term for generic name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of generic names</returns>
    /// <response code="200">Returns list of generic names</response>
    [HttpGet("generic-names")]
    [ProducesResponseType(typeof(List<GenericNameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGenericNames([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = new GetAllGenericNamesQuery(search);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            genericNames => Ok(genericNames),
            Problem);
    }
}