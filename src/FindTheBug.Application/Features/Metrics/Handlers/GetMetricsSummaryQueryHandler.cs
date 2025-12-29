using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Features.Metrics.DTOs;
using FindTheBug.Application.Features.Metrics.Queries;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Metrics.Handlers;

public class GetMetricsSummaryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetMetricsSummaryQuery, ErrorOr<MetricsSummaryDto>>
{
    public async Task<ErrorOr<MetricsSummaryDto>> Handle(GetMetricsSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var thisMonth = new DateTime(today.Year, today.Month, 1);

        var totalPatients = await unitOfWork.Repository<LabReceipt>().GetQueryable().CountAsync(cancellationToken);
        var todayPatients = await unitOfWork.Repository<LabReceipt>().GetQueryable()
            .Where(p => p.CreatedAt.Date == today)
            .CountAsync(cancellationToken);

        var totalTests = await unitOfWork.Repository<ReceiptTest>().GetQueryable().CountAsync(cancellationToken);
        var pendingTests = await unitOfWork.Repository<ReceiptTest>().GetQueryable()
            .Where(te => te.Status == ReceiptTestStatus.Pending)
            .CountAsync(cancellationToken);

        var todayRevenue = await unitOfWork.Repository<Invoice>().GetQueryable()
            .Where(i => i.InvoiceDate.Date == today)
            .SumAsync(i => (decimal?)i.TotalAmount, cancellationToken) ?? 0;

        var monthRevenue = await unitOfWork.Repository<Invoice>().GetQueryable()
            .Where(i => i.InvoiceDate >= thisMonth)
            .SumAsync(i => (decimal?)i.TotalAmount, cancellationToken) ?? 0;

        return new MetricsSummaryDto
        {
            TotalPatients = totalPatients,
            TodayPatients = todayPatients,
            TotalTests = totalTests,
            PendingTests = pendingTests,
            TodayRevenue = todayRevenue,
            MonthRevenue = monthRevenue
        };
    }
}
