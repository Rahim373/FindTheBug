using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.DTOs;

namespace FindTheBug.Application.Features.Roles.Queries;

public record GetRoleByIdQuery(Guid Id) : IQuery<RoleResponseDto>;
