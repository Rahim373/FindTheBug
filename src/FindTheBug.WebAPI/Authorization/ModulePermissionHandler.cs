using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FindTheBug.WebAPI.Authorization;

/// <summary>
/// Authorization handler for module-based permissions
/// </summary>
public class ModulePermissionHandler : AuthorizationHandler<ModulePermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ModulePermissionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModulePermissionRequirement requirement)
    {
        // Check if user is authenticated
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        // Create a scope to access scoped services
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var modulePermissionService = scope.ServiceProvider.GetRequiredService<FindTheBug.Application.Common.Interfaces.IModulePermissionService>();

            // Check if user has the required module permission
            var hasPermission = await modulePermissionService.HasPermissionAsync(
                userId,
                requirement.ModuleName,
                requirement.RequiredPermission
            );

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}