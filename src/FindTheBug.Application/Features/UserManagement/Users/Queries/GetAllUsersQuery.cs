using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Users.Queries;

public record GetAllUsersQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<UserListItemDto>>;
