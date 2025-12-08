using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Roles.DTOs;

namespace FindTheBug.Application.Features.Roles.Queries;

public record GetAllRolesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null
) : IQuery<PagedResult<RoleListItemDto>>;
