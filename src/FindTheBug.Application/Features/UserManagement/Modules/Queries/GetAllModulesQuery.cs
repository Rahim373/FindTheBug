using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Modules.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Modules.Queries;

public record GetAllModulesQuery() : IQuery<List<ModuleDto>>;
