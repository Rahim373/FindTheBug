using FindTheBug.Application.Features.Doctors.Commands;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Doctor management endpoints
/// </summary>
public class DoctorsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all doctors with optional search
    /// </summary>
    /// <param name="search">Search by name, phone number, degree, office, or speciality</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of doctors</returns>
    /// <response code="200">Returns the list of doctors</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DoctorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = new GetAllDoctorsQuery(search);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            doctors => Ok(doctors),
            Problem);
    }

    /// <summary>
    /// Get doctor by ID
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Doctor details</returns>
    /// <response code="200">Returns the doctor</response>
    /// <response code="404">If the doctor is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDoctorByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            doctor => Ok(doctor),
            Problem);
    }

    /// <summary>
    /// Create new doctor
    /// </summary>
    /// <param name="command">Doctor creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created doctor</returns>
    /// <response code="200">Returns the newly created doctor</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If a doctor with the same phone number already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            doctor => Ok(doctor),
            Problem);
    }

    /// <summary>
    /// Update existing doctor
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <param name="command">Doctor update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated doctor</returns>
    /// <response code="200">Returns the updated doctor</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the doctor is not found</response>
    /// <response code="409">If another doctor with the same phone number already exists</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await mediator.Send(updateCommand, cancellationToken);

        return result.Match(
            doctor => Ok(doctor),
            Problem);
    }

    /// <summary>
    /// Delete doctor
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Doctor deleted successfully</response>
    /// <response code="404">If the doctor is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDoctorCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}
