namespace FindTheBug.Application.Features.Metrics.Contracts;

public record MetricsSummaryDto(
    string Message,
    string MetricsEndpoint,
    string HealthEndpoint
);
