using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.TestEntries.Commands;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test entry management - register patients for tests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestEntriesController(IMediator mediator, IUnitOfWork unitOfWork, ILogger<TestEntriesController> logger) : ControllerBase
{
    /// <summary>
    /// Get all test entries with optional status filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<TestEntry>>>> GetAll([FromQuery] string? status, CancellationToken cancellationToken)
    {
        try
        {
            var entries = await unitOfWork.Repository<TestEntry>().GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(status))
            {
                entries = entries.Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(Result<IEnumerable<TestEntry>>.Success(entries.OrderByDescending(e => e.EntryDate)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving test entries");
            return StatusCode(500, Result<IEnumerable<TestEntry>>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Get test entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<TestEntry>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entry = await unitOfWork.Repository<TestEntry>().GetByIdAsync(id, cancellationToken);
            if (entry is null)
                return NotFound(Result<TestEntry>.Failure($"Test entry with ID {id} not found"));

            return Ok(Result<TestEntry>.Success(entry));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving test entry {Id}", id);
            return StatusCode(500, Result<TestEntry>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Register patient for test
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<TestEntry>>> Create([FromBody] CreateTestEntryCommand command, CancellationToken cancellationToken)
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
            logger.LogError(ex, "Error creating test entry");
            return StatusCode(500, Result<TestEntry>.Failure("An error occurred"));
        }
    }
}
