using MediatR;

namespace FindTheBug.Application.Features.Modules.Queries;

public record GetAllModulesQuery : IRequest<List<ModuleDto>>;

public record ModuleDto(
    Guid Id,
    string Name,
    string? DisplayName,
    string? Description,
    bool IsActive
);
