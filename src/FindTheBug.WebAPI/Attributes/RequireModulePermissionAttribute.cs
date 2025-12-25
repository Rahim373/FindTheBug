using FindTheBug.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace FindTheBug.WebAPI.Attributes;

/// <summary>
/// Attribute to specify required module permission for an endpoint
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireModulePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Module name (e.g., "Doctors", "Patients", ModuleConstants.Billing)
    /// </summary>
    public string ModuleName { get; }

    /// <summary>
    /// Required permission for the module
    /// </summary>
    public ModulePermission Permission { get; }

    /// <summary>
    /// Creates a new instance of RequireModulePermissionAttribute
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <param name="permission">Required permission level</param>
    public RequireModulePermissionAttribute(string moduleName, ModulePermission permission)
    {
        ModuleName = moduleName;
        Permission = permission;
        Policy = $"Module_{moduleName}_{permission}";
    }
}