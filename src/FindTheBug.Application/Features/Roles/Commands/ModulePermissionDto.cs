namespace FindTheBug.Application.Features.Roles.Commands;

public record ModulePermissionDto(
    Guid ModuleId,
    bool CanView,
    bool CanCreate,
    bool CanEdit,
    bool CanDelete
);
