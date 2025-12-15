using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Roles.Queries;

public record GetRoleByIdQuery(Guid Id) : IQuery<RoleResponseDto>;
