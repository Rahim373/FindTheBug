using FindTheBug.Domain.Common;
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
            // Add dynamic policies for modules
            // These policies will be evaluated at runtime based on module name and permission
            options.AddPolicy("Module_Doctors_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Doctors", ModulePermission.View)));

            options.AddPolicy("Module_Doctors_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Doctors", ModulePermission.Create)));

            options.AddPolicy("Module_Doctors_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Doctors", ModulePermission.Edit)));

            options.AddPolicy("Module_Doctors_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Doctors", ModulePermission.Delete)));

            options.AddPolicy("Module_Patients_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Patients", ModulePermission.View)));

            options.AddPolicy("Module_Patients_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Patients", ModulePermission.Create)));

            options.AddPolicy("Module_Patients_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Patients", ModulePermission.Edit)));

            options.AddPolicy("Module_Patients_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Patients", ModulePermission.Delete)));

            options.AddPolicy("Module_Laboratory_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Laboratory", ModulePermission.View)));

            options.AddPolicy("Module_Laboratory_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Laboratory", ModulePermission.Create)));

            options.AddPolicy("Module_Laboratory_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Laboratory", ModulePermission.Edit)));

            options.AddPolicy("Module_Laboratory_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Laboratory", ModulePermission.Delete)));

            options.AddPolicy("Module_Dispensary_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Dispensary", ModulePermission.View)));

            options.AddPolicy("Module_Dispensary_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Dispensary", ModulePermission.Create)));

            options.AddPolicy("Module_Dispensary_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Dispensary", ModulePermission.Edit)));

            options.AddPolicy("Module_Dispensary_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Dispensary", ModulePermission.Delete)));

            options.AddPolicy("Module_Billing_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Billing", ModulePermission.View)));

            options.AddPolicy("Module_Billing_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Billing", ModulePermission.Create)));

            options.AddPolicy("Module_Billing_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Billing", ModulePermission.Edit)));

            options.AddPolicy("Module_Billing_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("Billing", ModulePermission.Delete)));

            options.AddPolicy("Module_UserManagement_View", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("UserManagement", ModulePermission.View)));

            options.AddPolicy("Module_UserManagement_Create", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("UserManagement", ModulePermission.Create)));

            options.AddPolicy("Module_UserManagement_Edit", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("UserManagement", ModulePermission.Edit)));

            options.AddPolicy("Module_UserManagement_Delete", policy =>
                policy.Requirements.Add(new ModulePermissionRequirement("UserManagement", ModulePermission.Delete)));
        });

        return services;
    }
}