using FindTheBug.Domain.Common;

namespace FindTheBug.Application.Common.Interfaces;

/// <summary>
/// Service to check module permissions for users
/// </summary>
public interface IModulePermissionService
{
    /// <summary>
    /// Checks if a user has the specified permission for a module
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="moduleName">Module name</param>
    /// <param name="requiredPermission">Required permission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has permission, false otherwise</returns>
    Task<bool> HasPermissionAsync(Guid userId, string moduleName, ModulePermission requiredPermission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all module permissions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of module names and their permissions</returns>
    Task<Dictionary<string, ModulePermission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}