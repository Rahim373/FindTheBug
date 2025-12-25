using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Queries;

public record GetDrugByIdQuery(Guid Id) : IQuery<DrugResponseDto>;
