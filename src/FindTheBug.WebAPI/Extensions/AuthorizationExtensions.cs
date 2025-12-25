using FindTheBug.Domain.Common;
using FindTheBug.Domain.Contracts;
using FindTheBug.WebAPI.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace FindTheBug.WebAPI.Extensions;

/// <summary>
/// Extension methods for configuring authorization
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds module-based authorization policies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddModuleAuthorization(this IServiceCollection services)
    {
        // Register the module permission handler
        services.AddSingleton<IAuthorizationHandler, ModulePermissionHandler>();

        // Configure authorization with module policies
        services.AddAuthorization(options =>
        {
            var permissions = new[]
            {
                ModulePermission.View,
                ModulePermission.Create,
                ModulePermission.Edit,
                ModulePermission.Delete
            };

            foreach (var permission in permissions)
            {
                foreach (var module in ModuleConstants.GetModules())
                {
                    // Add dynamic policies for modules
                    // These policies will be evaluated at runtime based on module name and permission
                    options.AddPolicy($"Module_{module}_{permission.ToString()}", policy =>
                        policy.Requirements.Add(new ModulePermissionRequirement(module, permission)));
                }
            }
        });

        return services;
    }
}