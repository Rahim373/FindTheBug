using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Metrics.Contracts;
using FindTheBug.Application.Features.Metrics.Queries;

namespace FindTheBug.Application.Features.Metrics.Handlers;

public class GetMetricsSummaryQueryHandler(
    IMetricsService metricsService) 
    : IQueryHandler<GetMetricsSummaryQuery, MetricsSummaryDto>
{
    public async Task<ErrorOr<MetricsSummaryDto>> Handle(GetMetricsSummaryQuery request, CancellationToken cancellationToken)
    {
        return new MetricsSummaryDto(
            Message: "Metrics are being collected. Access /metrics endpoint for Prometheus format.",
            MetricsEndpoint: "/metrics",
            HealthEndpoint: "/health"
        );
    }
}
