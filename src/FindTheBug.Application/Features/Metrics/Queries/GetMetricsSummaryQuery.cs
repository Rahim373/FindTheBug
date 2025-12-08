using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Metrics.DTOs;

namespace FindTheBug.Application.Features.Metrics.Queries;

public record GetMetricsSummaryQuery() : IQuery<MetricsSummaryDto>;
