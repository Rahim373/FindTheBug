using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Metrics.Contracts;
using FindTheBug.Application.Features.Metrics.Queries;

namespace FindTheBug.Application.Features.Metrics.Handlers;

public class GetMetricsSummaryQueryHandler(
    IMetricsService metricsService,
    ITenantService tenantService) 
    : IQueryHandler<GetMetricsSummaryQuery, MetricsSummaryDto>
{
    public async Task<ErrorOr<MetricsSummaryDto>> Handle(GetMetricsSummaryQuery request, CancellationToken cancellationToken)
    {
        var tenants = await tenantService.GetAllTenantsAsync(cancellationToken);
        var tenantCount = tenants.Count();
        
        metricsService.UpdateActiveTenantCount(tenantCount);

        return new MetricsSummaryDto(
            Message: "Metrics are being collected. Access /metrics endpoint for Prometheus format.",
            ActiveTenants: tenantCount,
            MetricsEndpoint: "/metrics",
            HealthEndpoint: "/health"
        );
    }
}
