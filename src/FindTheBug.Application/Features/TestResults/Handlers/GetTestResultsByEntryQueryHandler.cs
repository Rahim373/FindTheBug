using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestResults.DTOs;
using FindTheBug.Application.Features.TestResults.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.TestResults.Handlers;

public class GetTestResultsByEntryQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetTestResultsByEntryQuery, List<TestResultResponseDto>>
{
    public async Task<ErrorOr<List<TestResultResponseDto>>> Handle(GetTestResultsByEntryQuery request, CancellationToken cancellationToken)
    {
        var results = await unitOfWork.Repository<TestResult>().GetQueryable()
            .Include(tr => tr.TestParameter)
            .Where(tr => tr.TestEntryId == request.TestEntryId)
            .OrderBy(tr => tr.TestParameter.DisplayOrder)
            .ToListAsync(cancellationToken);

        var dtos = results.Select(r => new TestResultResponseDto
        {
            Id = r.Id,
            TestEntryId = r.TestEntryId,
            TestParameterId = r.TestParameterId,
            ParameterName = r.TestParameter?.ParameterName ?? string.Empty,
            ResultValue = r.ResultValue,
            IsAbnormal = r.IsAbnormal,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        }).ToList();

        return dtos;
    }
}
