using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Application.Features.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Patient management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get all patients with optional search
    /// </summary>
    /// <param name="search">Search by name or mobile number</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = new GetAllPatientsQuery(search);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Register new patient (mobile number required)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
