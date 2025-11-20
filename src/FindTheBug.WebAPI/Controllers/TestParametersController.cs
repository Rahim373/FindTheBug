using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test parameter management for diagnostic tests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestParametersController(IUnitOfWork unitOfWork, ILogger<TestParametersController> logger) : ControllerBase
{
    /// <summary>
    /// Get all parameters for a diagnostic test
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<TestParameter>>>> GetAll([FromQuery] Guid? diagnosticTestId, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = await unitOfWork.Repository<TestParameter>().GetAllAsync(cancellationToken);
            
            if (diagnosticTestId.HasValue)
            {
                parameters = parameters.Where(p => p.DiagnosticTestId == diagnosticTestId.Value);
            }

            return Ok(Result<IEnumerable<TestParameter>>.Success(parameters.OrderBy(p => p.DisplayOrder)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving test parameters");
            return StatusCode(500, Result<IEnumerable<TestParameter>>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Add parameter to diagnostic test
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<TestParameter>>> Create([FromBody] TestParameter parameter, CancellationToken cancellationToken)
    {
        try
        {
            parameter.Id = Guid.NewGuid();
            var created = await unitOfWork.Repository<TestParameter>().AddAsync(parameter, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { diagnosticTestId = created.DiagnosticTestId }, Result<TestParameter>.Success(created));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating test parameter");
            return StatusCode(500, Result<TestParameter>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Update test parameter
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<TestParameter>>> Update(Guid id, [FromBody] TestParameter parameter, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await unitOfWork.Repository<TestParameter>().GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(Result<TestParameter>.Failure($"Parameter with ID {id} not found"));

            parameter.Id = id;
            await unitOfWork.Repository<TestParameter>().UpdateAsync(parameter, cancellationToken);
            return Ok(Result<TestParameter>.Success(parameter));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating parameter {Id}", id);
            return StatusCode(500, Result<TestParameter>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Delete test parameter
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.Repository<TestParameter>().DeleteAsync(id, cancellationToken);
            return Ok(Result<bool>.Success(true));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting parameter {Id}", id);
            return StatusCode(500, Result<bool>.Failure("An error occurred"));
        }
    }
}
