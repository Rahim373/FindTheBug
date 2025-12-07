namespace FindTheBug.Application.Common.Interfaces;

public interface IMetricsService
{
    void RecordOperationDuration(string operation, double durationSeconds);
    void TrackEntityOperation(string entityType, string operation);
}
