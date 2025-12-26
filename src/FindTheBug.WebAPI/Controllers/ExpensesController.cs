using FindTheBug.Application.Features.Expenses.Commands;
using FindTheBug.Application.Features.Expenses.DTOs;
using FindTheBug.Application.Features.Expenses.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Expense management endpoints
/// </summary>
[Authorize]
public class ExpensesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all expenses with optional filters
    /// </summary>
    /// <param name="department">Filter by department (Lab/Dispensary)</param>
    /// <param name="search">Search by note or reference number</param>
    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Page size for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of expenses</returns>
    /// <response code="200">Returns the list of expenses</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Accounts, ModulePermission.View)]
    [RequireModulePermission(ModuleConstants.Dispensary, ModulePermission.View)]
    [ProducesResponseType(typeof(PaginatedExpenseListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? department,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesQuery(department, search, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            expenses => Ok(expenses),
            Problem);
    }

    /// <summary>
    /// Get expense by ID
    /// </summary>
    /// <param name="id">Expense ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Expense details</returns>
    /// <response code="200">Returns the expense</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the expense is not found</response>
    [HttpGet("{id}")]
    [RequireModulePermission(ModuleConstants.Accounts, ModulePermission.View)]
    [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetExpenseByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            expense => Ok(expense),
            Problem);
    }

    /// <summary>
    /// Create new expense
    /// </summary>
    /// <param name="command">Expense creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created expense</returns>
    /// <response code="200">Returns the newly created expense</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Accounts, ModulePermission.Create)]
    [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            expense => Ok(expense),
            Problem);
    }

    /// <summary>
    /// Update existing expense
    /// </summary>
    /// <param name="command">Expense update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated expense</returns>
    /// <response code="200">Returns the updated expense</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the expense is not found</response>
    [HttpPut]
    [RequireModulePermission(ModuleConstants.Accounts, ModulePermission.Edit)]
    [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateExpenseCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            expense => Ok(expense),
            Problem);
    }

    /// <summary>
    /// Delete expense
    /// </summary>
    /// <param name="id">Expense ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Expense deleted successfully</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the expense is not found</response>
    [HttpDelete("{id}")]
    [RequireModulePermission(ModuleConstants.Accounts, ModulePermission.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}