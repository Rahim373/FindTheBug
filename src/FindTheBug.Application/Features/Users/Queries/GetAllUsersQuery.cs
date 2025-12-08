using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Users.DTOs;

namespace FindTheBug.Application.Features.Users.Queries;

public record GetAllUsersQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<UserListItemDto>>;
