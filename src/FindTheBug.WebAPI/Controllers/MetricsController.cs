using FindTheBug.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController(IMetricsService metricsService, ITenantService tenantService) : ControllerBase
{
    /// <summary>
    /// Get current metrics summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetMetricsSummary(CancellationToken cancellationToken)
    {
        var tenants = await tenantService.GetAllTenantsAsync(cancellationToken);
        var tenantCount = tenants.Count();
        
        metricsService.UpdateActiveTenantCount(tenantCount);

        return Ok(new
        {
            Message = "Metrics are being collected. Access /metrics endpoint for Prometheus format.",
            ActiveTenants = tenantCount,
            MetricsEndpoint = "/metrics",
            HealthEndpoint = "/health"
        });
    }
}
