using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Common;
using FindTheBug.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Services;

/// <summary>
/// Service to check module permissions for users
/// </summary>
public class ModulePermissionService : IModulePermissionService
{
    private readonly ApplicationDbContext _context;

    public ModulePermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks if a user has the specified permission for a module
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid userId, string moduleName, ModulePermission requiredPermission, CancellationToken cancellationToken = default)
    {
        // Get all user's roles
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        if (!userRoles.Any())
        {
            return false;
        }

        // Get module by name
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.Name == moduleName && m.IsActive, cancellationToken);

        if (module == null)
        {
            return false;
        }

        // Get all role module permissions for user's roles and the module
        var rolePermissions = await _context.RoleModulePermissions
            .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.ModuleId == module.Id)
            .ToListAsync(cancellationToken);

        if (!rolePermissions.Any())
        {
            return false;
        }

        // Calculate combined permissions from all roles
        ModulePermission combinedPermissions = ModulePermission.None;

        foreach (var rolePermission in rolePermissions)
        {
            if (rolePermission.CanView)
                combinedPermissions |= ModulePermission.View;
            if (rolePermission.CanCreate)
                combinedPermissions |= ModulePermission.Create;
            if (rolePermission.CanEdit)
                combinedPermissions |= ModulePermission.Edit;
            if (rolePermission.CanDelete)
                combinedPermissions |= ModulePermission.Delete;
        }

        // Check if user has the required permission
        return (combinedPermissions & requiredPermission) == requiredPermission;
    }

    /// <summary>
    /// Gets all module permissions for a user
    /// </summary>
    public async Task<Dictionary<string, ModulePermission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Get all user's roles
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        if (!userRoles.Any())
        {
            return new Dictionary<string, ModulePermission>();
        }

        // Get all role module permissions for user's roles
        var rolePermissions = await _context.RoleModulePermissions
            .Include(rmp => rmp.Module)
            .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.Module.IsActive)
            .ToListAsync(cancellationToken);

        // Aggregate permissions by module
        var permissions = new Dictionary<string, ModulePermission>();

        foreach (var rolePermission in rolePermissions)
        {
            var moduleName = rolePermission.Module.Name;
            
            if (!permissions.ContainsKey(moduleName))
            {
                permissions[moduleName] = ModulePermission.None;
            }

            if (rolePermission.CanView)
                permissions[moduleName] |= ModulePermission.View;
            if (rolePermission.CanCreate)
                permissions[moduleName] |= ModulePermission.Create;
            if (rolePermission.CanEdit)
                permissions[moduleName] |= ModulePermission.Edit;
            if (rolePermission.CanDelete)
                permissions[moduleName] |= ModulePermission.Delete;
        }

        return permissions;
    }
}