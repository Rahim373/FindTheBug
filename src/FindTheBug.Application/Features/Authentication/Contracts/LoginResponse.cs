namespace FindTheBug.Application.Features.Authentication.Contracts;

public record ModulePermissionInfo(
    string Module,
    string Permission
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Roles,
    List<ModulePermissionInfo> Permissions
);