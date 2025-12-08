using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Modules.DTOs;

namespace FindTheBug.Application.Features.Modules.Queries;

public record GetAllModulesQuery() : IQuery<List<ModuleDto>>;
