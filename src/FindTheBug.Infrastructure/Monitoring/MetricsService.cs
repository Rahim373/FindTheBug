using FindTheBug.Application.Common.Interfaces;
using Prometheus;

namespace FindTheBug.Infrastructure.Monitoring;

public class MetricsService : IMetricsService
{
    private static readonly Counter TenantRequestsCounter = Metrics.CreateCounter(
        "findthebug_tenant_requests_total",
        "Total number of requests per tenant",
        new CounterConfiguration { LabelNames = new[] { "tenant_id" } });

    private static readonly Histogram OperationDurationHistogram = Metrics.CreateHistogram(
        "findthebug_operation_duration_seconds",
        "Duration of operations in seconds",
        new HistogramConfiguration { LabelNames = new[] { "operation" } });

    private static readonly Counter EntityOperationsCounter = Metrics.CreateCounter(
        "findthebug_entity_operations_total",
        "Total number of entity operations",
        new CounterConfiguration { LabelNames = new[] { "entity_type", "operation" } });

    private static readonly Gauge ActiveTenantsGauge = Metrics.CreateGauge(
        "findthebug_active_tenants",
        "Number of active tenants");

    public void IncrementTenantRequests(string tenantId)
    {
        TenantRequestsCounter.WithLabels(tenantId).Inc();
    }

    public void RecordOperationDuration(string operation, double durationSeconds)
    {
        OperationDurationHistogram.WithLabels(operation).Observe(durationSeconds);
    }

    public void TrackEntityOperation(string entityType, string operation)
    {
        EntityOperationsCounter.WithLabels(entityType, operation).Inc();
    }

    public void UpdateActiveTenantCount(int count)
    {
        ActiveTenantsGauge.Set(count);
    }
}
