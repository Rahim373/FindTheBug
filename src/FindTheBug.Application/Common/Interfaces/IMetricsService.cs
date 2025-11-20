namespace FindTheBug.Application.Common.Interfaces;

public interface IMetricsService
{
    void IncrementTenantRequests(string tenantId);
    void RecordOperationDuration(string operation, double durationSeconds);
    void TrackEntityOperation(string entityType, string operation);
    void UpdateActiveTenantCount(int count);
}
