using FindTheBug.Application.Common.Interfaces;
using Prometheus;

namespace FindTheBug.Infrastructure.Monitoring;

public class MetricsService : IMetricsService
{
    private static readonly Histogram OperationDurationHistogram = Metrics.CreateHistogram(
        "findthebug_operation_duration_seconds",
        "Duration of operations in seconds",
        new HistogramConfiguration { LabelNames = new[] { "operation" } });

    private static readonly Counter EntityOperationsCounter = Metrics.CreateCounter(
        "findthebug_entity_operations_total",
        "Total number of entity operations",
        new CounterConfiguration { LabelNames = new[] { "entity_type", "operation" } });
    public void RecordOperationDuration(string operation, double durationSeconds)
    {
        OperationDurationHistogram.WithLabels(operation).Observe(durationSeconds);
    }

    public void TrackEntityOperation(string entityType, string operation)
    {
        EntityOperationsCounter.WithLabels(entityType, operation).Inc();
    }
}
