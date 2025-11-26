using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Metrics.Contracts;

namespace FindTheBug.Application.Features.Metrics.Queries;

public record GetMetricsSummaryQuery : IQuery<MetricsSummaryDto>;
