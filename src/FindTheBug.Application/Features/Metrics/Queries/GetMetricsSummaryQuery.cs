using ErrorOr;
using FindTheBug.Application.Features.Metrics.DTOs;
using MediatR;

namespace FindTheBug.Application.Features.Metrics.Queries;

public record GetMetricsSummaryQuery() : IRequest<ErrorOr<MetricsSummaryDto>>;
