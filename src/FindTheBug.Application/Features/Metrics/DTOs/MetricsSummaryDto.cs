namespace FindTheBug.Application.Features.Metrics.DTOs;

public record MetricsSummaryDto
{
    public int TotalPatients { get; init; }
    public int TodayPatients { get; init; }
    public int TotalTests { get; init; }
    public int PendingTests { get; init; }
    public decimal TodayRevenue { get; init; }
    public decimal MonthRevenue { get; init; }
}
