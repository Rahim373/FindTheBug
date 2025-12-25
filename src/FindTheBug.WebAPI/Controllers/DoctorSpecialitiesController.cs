using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Doctor specialities management endpoints
/// </summary>
public class DoctorSpecialitiesController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get all active doctor specialities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of doctor specialities</returns>
    /// <response code="200">Returns the list of doctor specialities</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpGet]
    [RequireModulePermission(ModuleConstants.Doctors, ModulePermission.View)]
    [ProducesResponseType(typeof(List<DoctorSpecialityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetDoctorSpecialitiesQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            specialities => Ok(specialities),
            Problem);
    }
}
