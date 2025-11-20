using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test results management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestResultsController(IUnitOfWork unitOfWork, ILogger<TestResultsController> logger) : ControllerBase
{
    /// <summary>
    /// Record test results for a test entry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<TestResult>>> Create([FromBody] TestResult result, CancellationToken cancellationToken)
    {
        try
        {
            result.Id = Guid.NewGuid();
            result.ResultDate = DateTime.UtcNow;
            
            var created = await unitOfWork.Repository<TestResult>().AddAsync(result, cancellationToken);
            return CreatedAtAction(nameof(GetByTestEntry), new { testEntryId = created.TestEntryId }, Result<TestResult>.Success(created));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating test result");
            return StatusCode(500, Result<TestResult>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Get all results for a test entry
    /// </summary>
    [HttpGet("entry/{testEntryId}")]
    public async Task<ActionResult<Result<IEnumerable<TestResult>>>> GetByTestEntry(Guid testEntryId, CancellationToken cancellationToken)
    {
        try
        {
            var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
            var entryResults = results.Where(r => r.TestEntryId == testEntryId);
            
            return Ok(Result<IEnumerable<TestResult>>.Success(entryResults));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving test results for entry {EntryId}", testEntryId);
            return StatusCode(500, Result<IEnumerable<TestResult>>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Update test result
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<TestResult>>> Update(Guid id, [FromBody] TestResult result, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await unitOfWork.Repository<TestResult>().GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(Result<TestResult>.Failure($"Result with ID {id} not found"));

            result.Id = id;
            await unitOfWork.Repository<TestResult>().UpdateAsync(result, cancellationToken);
            return Ok(Result<TestResult>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating test result {Id}", id);
            return StatusCode(500, Result<TestResult>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Verify test results
    /// </summary>
    [HttpPost("{testEntryId}/verify")]
    public async Task<ActionResult<Result<bool>>> VerifyResults(Guid testEntryId, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
            var entryResults = results.Where(r => r.TestEntryId == testEntryId).ToList();

            foreach (var result in entryResults)
            {
                result.VerifiedBy = request.VerifiedBy;
                result.VerifiedDate = DateTime.UtcNow;
                await unitOfWork.Repository<TestResult>().UpdateAsync(result, cancellationToken);
            }

            return Ok(Result<bool>.Success(true));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying test results for entry {EntryId}", testEntryId);
            return StatusCode(500, Result<bool>.Failure("An error occurred"));
        }
    }
}

public record VerifyRequest(string VerifiedBy);
