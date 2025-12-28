using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Diagnostic test management endpoints
/// </summary>
[Authorize]
public class DiagnosticTestsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all diagnostic tests with optional search, category, and pagination
    /// </summary>
    /// <param name="search">Search by test name, test code, or category</param>
    /// <param name="category">Filter by category</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of diagnostic tests</returns>
    /// <response code="200">Returns paginated list of diagnostic tests</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.View)]
    [ProducesResponseType(typeof(PagedResult<DiagnosticTestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken,
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllDiagnosticTestsQuery(search, category, isActive, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            tests => Ok(tests),
            Problem);
    }

    /// <summary>
    /// Get diagnostic test by ID
    /// </summary>
    /// <param name="id">Diagnostic test ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Diagnostic test details</returns>
    /// <response code="200">Returns diagnostic test</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If diagnostic test is not found</response>
    [HttpGet("{id}")]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.View)]
    [ProducesResponseType(typeof(DiagnosticTestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDiagnosticTestByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            test => Ok(test),
            Problem);
    }

    /// <summary>
    /// Create a new diagnostic test
    /// </summary>
    /// <param name="command">Diagnostic test creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created diagnostic test</returns>
    /// <response code="200">Returns the newly created diagnostic test</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="409">If a diagnostic test with the same test code already exists</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.Create)]
    [ProducesResponseType(typeof(DiagnosticTestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDiagnosticTestCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            test => Ok(test),
            Problem);
    }

    /// <summary>
    /// Update existing diagnostic test
    /// </summary>
    /// <param name="id">Diagnostic test ID</param>
    /// <param name="command">Diagnostic test update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated diagnostic test</returns>
    /// <response code="200">Returns updated diagnostic test</response>
    /// <response code="400">If request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If diagnostic test is not found</response>
    /// <response code="409">If another diagnostic test with the same test code already exists</response>
    [HttpPut("{id}")]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.Edit)]
    [ProducesResponseType(typeof(DiagnosticTestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiagnosticTestCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await mediator.Send(updateCommand, cancellationToken);

        return result.Match(
            test => Ok(test),
            Problem);
    }

    /// <summary>
    /// Delete diagnostic test
    /// </summary>
    /// <param name="id">Diagnostic test ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Diagnostic test deleted successfully</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If diagnostic test is not found</response>
    [HttpDelete("{id}")]
    [RequireModulePermission(ModuleConstants.Laboratory, ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDiagnosticTestCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}