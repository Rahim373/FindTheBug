using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Application.Features.Dispensary.Drugs.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class GetAllBrandsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllBrandsQuery, List<BrandDto>>
{
    public async Task<ErrorOr<Result<List<BrandDto>>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await unitOfWork.Repository<Brand>().GetQueryable()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                IsActive = b.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<List<BrandDto>>.Success(brands);
    }
}
