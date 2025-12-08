using FindTheBug.Application.Features.Metrics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

public class MetricsController(ISender mediator) : BaseApiController
{
    /// <summary>
    /// Get current metrics summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetMetricsSummary(CancellationToken cancellationToken)
    {
        var query = new GetMetricsSummaryQuery();
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            metrics => Ok(metrics),
            errors => Problem(errors));
    }
}
