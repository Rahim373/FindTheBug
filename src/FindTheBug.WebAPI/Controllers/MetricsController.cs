using FindTheBug.Application.Features.Metrics.DTOs;
using FindTheBug.Application.Features.Metrics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Metrics and dashboard endpoints
/// </summary>
public class MetricsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get dashboard metrics summary
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Metrics summary including patient count, test statistics, and revenue</returns>
    /// <response code="200">Returns the metrics summary</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(MetricsSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var query = new GetMetricsSummaryQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            metrics => Ok(metrics),
            Problem);
    }
}
