using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Drugs.Commands;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class CreateDrugCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDrugCommand, DrugResponseDto>
{
    public async Task<ErrorOr<Result<DrugResponseDto>>> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        // Validate GenericName exists
        var genericName = await unitOfWork.Repository<GenericName>().GetQueryable()
            .FirstOrDefaultAsync(gn => gn.Id == request.GenericNameId && gn.IsActive, cancellationToken);

        if (genericName == null)
            return Error.NotFound("Drug.GenericNameNotFound", "Generic name not found");

        // Validate Brand exists
        var brand = await unitOfWork.Repository<Brand>().GetQueryable()
            .FirstOrDefaultAsync(b => b.Id == request.BrandId && b.IsActive, cancellationToken);

        if (brand == null)
            return Error.NotFound("Drug.BrandNotFound", "Brand not found");

        var drug = new Drug
        {
            Name = request.Name,
            Strength = request.Strength,
            GenericNameId = request.GenericNameId,
            BrandId = request.BrandId,
            Type = request.Type,
            UnitPrice = request.UnitPrice,
            PhotoPath = request.PhotoPath,
            IsActive = true
        };

        var created = await unitOfWork.Repository<Drug>().AddAsync(drug, cancellationToken);

        // Get the created drug with related entities
        var drugWithRelations = await unitOfWork.Repository<Drug>().GetQueryable()
            .Include(d => d.GenericName)
            .Include(d => d.Brand)
            .FirstAsync(d => d.Id == created.Id, cancellationToken);

        return Result<DrugResponseDto>.Success(new DrugResponseDto
        {
            Id = drugWithRelations.Id,
            Name = drugWithRelations.Name,
            Strength = drugWithRelations.Strength,
            GenericName = new GenericNameDto
            {
                Id = drugWithRelations.GenericName.Id,
                Name = drugWithRelations.GenericName.Name,
                Description = drugWithRelations.GenericName.Description,
                IsActive = drugWithRelations.GenericName.IsActive
            },
            Brand = new BrandDto
            {
                Id = drugWithRelations.Brand.Id,
                Name = drugWithRelations.Brand.Name,
                IsActive = drugWithRelations.Brand.IsActive
            },
            Type = drugWithRelations.Type,
            UnitPrice = drugWithRelations.UnitPrice,
            PhotoPath = drugWithRelations.PhotoPath,
            IsActive = drugWithRelations.IsActive,
            CreatedAt = drugWithRelations.CreatedAt,
            UpdatedAt = drugWithRelations.UpdatedAt
        });
    }
}
