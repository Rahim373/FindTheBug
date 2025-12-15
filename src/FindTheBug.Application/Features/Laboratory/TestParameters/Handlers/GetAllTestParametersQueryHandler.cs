using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestParameters.DTOs;
using FindTheBug.Application.Features.Laboratory.TestParameters.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Handlers;

public class GetAllTestParametersQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllTestParametersQuery, List<TestParameterResponseDto>>
{
    public async Task<ErrorOr<List<TestParameterResponseDto>>> Handle(GetAllTestParametersQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<TestParameter>().GetQueryable();

        if (request.DiagnosticTestId.HasValue)
        {
            query = query.Where(tp => tp.DiagnosticTestId == request.DiagnosticTestId.Value);
        }

        var parameters = await query
            .OrderBy(tp => tp.DisplayOrder)
            .ToListAsync(cancellationToken);

        var dtos = parameters.Select(p => new TestParameterResponseDto
        {
            Id = p.Id,
            DiagnosticTestId = p.DiagnosticTestId,
            ParameterName = p.ParameterName,
            Unit = p.Unit,
            ReferenceRangeMin = p.ReferenceRangeMin,
            ReferenceRangeMax = p.ReferenceRangeMax,
            DataType = p.DataType,
            DisplayOrder = p.DisplayOrder,
            CreatedAt = p.CreatedAt
        }).ToList();

        return dtos;
    }
}
