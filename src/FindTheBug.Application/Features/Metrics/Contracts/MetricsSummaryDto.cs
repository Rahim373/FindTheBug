namespace FindTheBug.Application.Features.Metrics.Contracts;

public record MetricsSummaryDto(
    string Message,
    int ActiveTenants,
    string MetricsEndpoint,
    string HealthEndpoint
);
