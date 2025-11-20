using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test results management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestResultsController(IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    /// Record test results for a test entry
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestResult result, CancellationToken cancellationToken)
    {
        result.Id = Guid.NewGuid();
        result.ResultDate = DateTime.UtcNow;
        
        var created = await unitOfWork.Repository<TestResult>().AddAsync(result, cancellationToken);
        return CreatedAtAction(nameof(GetByTestEntry), new { testEntryId = created.TestEntryId }, created);
    }

    /// <summary>
    /// Get all results for a test entry
    /// </summary>
    [HttpGet("entry/{testEntryId}")]
    public async Task<IActionResult> GetByTestEntry(Guid testEntryId, CancellationToken cancellationToken)
    {
        var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
        var entryResults = results.Where(r => r.TestEntryId == testEntryId);
        
        return Ok(entryResults);
    }

    /// <summary>
    /// Update test result
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestResult result, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<TestResult>().GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        result.Id = id;
        await unitOfWork.Repository<TestResult>().UpdateAsync(result, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Verify test results
    /// </summary>
    [HttpPost("{testEntryId}/verify")]
    public async Task<IActionResult> VerifyResults(Guid testEntryId, [FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var results = await unitOfWork.Repository<TestResult>().GetAllAsync(cancellationToken);
        var entryResults = results.Where(r => r.TestEntryId == testEntryId).ToList();

        foreach (var result in entryResults)
        {
            result.VerifiedBy = request.VerifiedBy;
            result.VerifiedDate = DateTime.UtcNow;
            await unitOfWork.Repository<TestResult>().UpdateAsync(result, cancellationToken);
        }

        return Ok(true);
    }
}

public record VerifyRequest(string VerifiedBy);
