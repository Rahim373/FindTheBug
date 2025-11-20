using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Test parameters management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestParametersController(IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    /// Get all test parameters with optional filter by diagnostic test
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? diagnosticTestId, CancellationToken cancellationToken)
    {
        var parameters = await unitOfWork.Repository<TestParameter>().GetAllAsync(cancellationToken);
        
        if (diagnosticTestId.HasValue)
        {
            parameters = parameters.Where(p => p.DiagnosticTestId == diagnosticTestId.Value);
        }

        return Ok(parameters.OrderBy(p => p.DisplayOrder));
    }

    /// <summary>
    /// Create new test parameter
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestParameter parameter, CancellationToken cancellationToken)
    {
        parameter.Id = Guid.NewGuid();
        var created = await unitOfWork.Repository<TestParameter>().AddAsync(parameter, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { diagnosticTestId = created.DiagnosticTestId }, created);
    }

    /// <summary>
    /// Update test parameter
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestParameter parameter, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<TestParameter>().GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        parameter.Id = id;
        await unitOfWork.Repository<TestParameter>().UpdateAsync(parameter, cancellationToken);
        return Ok(parameter);
    }

    /// <summary>
    /// Delete test parameter
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await unitOfWork.Repository<TestParameter>().DeleteAsync(id, cancellationToken);
        return Ok(true);
    }
}
