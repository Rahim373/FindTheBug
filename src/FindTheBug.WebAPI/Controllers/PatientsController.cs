using FindTheBug.Application.Features.Patients.Commands;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Application.Features.Patients.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Patient management endpoints
/// </summary>
public class PatientsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all patients with optional search
    /// </summary>
    /// <param name="search">Search by name or mobile number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of patients</returns>
    /// <response code="200">Returns the list of patients</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Patient, ModulePermission.View)]
    [ProducesResponseType(typeof(List<PatientListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = new GetAllPatientsQuery(search);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            patients => Ok(patients),
            Problem);
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Patient details</returns>
    /// <response code="200">Returns the patient</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="404">If the patient is not found</response>
    [HttpGet("{id}")]
    [RequireModulePermission(ModuleConstants.Patient, ModulePermission.View)]
    [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            patient => Ok(patient),
            Problem);
    }

    /// <summary>
    /// Register new patient
    /// </summary>
    /// <param name="command">Patient registration request (mobile number required)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created patient</returns>
    /// <response code="200">Returns the newly registered patient</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    /// <response code="409">If a patient with the same mobile number already exists</response>
    [HttpPost]
    [RequireModulePermission(ModuleConstants.Patient, ModulePermission.Create)]
    [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            patient => Ok(patient),
            Problem);
    }
}