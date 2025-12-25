using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Application.Features.Dispensary.Drugs.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class GetDrugByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetDrugByIdQuery, DrugResponseDto>
{
    public async Task<ErrorOr<DrugResponseDto>> Handle(GetDrugByIdQuery request, CancellationToken cancellationToken)
    {
        var drug = await unitOfWork.Repository<Drug>().GetQueryable()
            .Include(d => d.GenericName)
            .Include(d => d.Brand)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (drug == null)
            return Error.NotFound("Drug.NotFound", "Drug not found");

        return new DrugResponseDto
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            GenericName = new GenericNameDto
            {
                Id = drug.GenericName.Id,
                Name = drug.GenericName.Name,
                Description = drug.GenericName.Description,
                IsActive = drug.GenericName.IsActive
            },
            Brand = new BrandDto
            {
                Id = drug.Brand.Id,
                Name = drug.Brand.Name,
                IsActive = drug.Brand.IsActive
            },
            Type = drug.Type,
            UnitPrice = drug.UnitPrice,
            PhotoPath = drug.PhotoPath,
            IsActive = drug.IsActive,
            CreatedAt = drug.CreatedAt,
            UpdatedAt = drug.UpdatedAt
        };
    }
}
