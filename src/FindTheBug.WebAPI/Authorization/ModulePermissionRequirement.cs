using FindTheBug.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace FindTheBug.WebAPI.Authorization;

/// <summary>
/// Requirement for module-based authorization
/// </summary>
public class ModulePermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Module name
    /// </summary>
    public string ModuleName { get; }

    /// <summary>
    /// Required permission for the module
    /// </summary>
    public ModulePermission RequiredPermission { get; }

    public ModulePermissionRequirement(string moduleName, ModulePermission requiredPermission)
    {
        ModuleName = moduleName;
        RequiredPermission = requiredPermission;
    }
}