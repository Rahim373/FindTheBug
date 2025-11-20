using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Application.Features.Patients.Queries;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Patient management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController(IMediator mediator, ILogger<PatientsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all patients with optional search
    /// </summary>
    /// <param name="search">Search by name or mobile number</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<Patient>>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllPatientsQuery(search);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.IsSuccess ? Ok(result) : StatusCode(500, result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving patients");
            return StatusCode(500, Result<IEnumerable<Patient>>.Failure("An error occurred while retrieving patients"));
        }
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<Patient>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetPatientByIdQuery(id);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving patient {Id}", id);
            return StatusCode(500, Result<Patient>.Failure("An error occurred"));
        }
    }

    /// <summary>
    /// Register new patient (mobile number required)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<Patient>>> Create([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
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
            logger.LogError(ex, "Error creating patient");
            return StatusCode(500, Result<Patient>.Failure("An error occurred while creating patient"));
        }
    }
}
