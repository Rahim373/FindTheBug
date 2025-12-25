using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Commands;

public record CreateDrugCommand(
    string Name,
    string Strength,
    Guid GenericNameId,
    Guid BrandId,
    FindTheBug.Domain.Entities.DrugType Type,
    decimal UnitPrice,
    string? PhotoPath
) : ICommand<DrugResponseDto>;
