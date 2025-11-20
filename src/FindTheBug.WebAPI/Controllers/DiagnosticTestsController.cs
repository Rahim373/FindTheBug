using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.DiagnosticTests.Commands;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FindTheBug.Application.Common.Interfaces;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Diagnostic test catalog management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiagnosticTestsController(IMediator mediator, IUnitOfWork unitOfWork, ILogger<DiagnosticTestsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all diagnostic tests with optional category filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<DiagnosticTest>>>> GetAll([FromQuery] string? category, CancellationToken cancellationToken)
    {
        try
        {
            var tests = await unitOfWork.Repository<DiagnosticTest>().GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(category))
            {
                tests = tests.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(Result<IEnumerable<DiagnosticTest>>.Success(tests.Where(t => t.IsActive)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving diagnostic tests");
            return StatusCode(500, Result<IEnumerable<DiagnosticTest>>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Get diagnostic test by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<DiagnosticTest>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var test = await unitOfWork.Repository<DiagnosticTest>().GetByIdAsync(id, cancellationToken);
            if (test is null)
                return NotFound(Result<DiagnosticTest>.Failure($"Test with ID {id} not found"));

            return Ok(Result<DiagnosticTest>.Success(test));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving test {Id}", id);
            return StatusCode(500, Result<DiagnosticTest>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Create new diagnostic test
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<DiagnosticTest>>> Create([FromBody] CreateDiagnosticTestCommand command, CancellationToken cancellationToken)
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
            logger.LogError(ex, "Error creating diagnostic test");
            return StatusCode(500, Result<DiagnosticTest>.Failure("An error occurred"));
        }
    }
}
