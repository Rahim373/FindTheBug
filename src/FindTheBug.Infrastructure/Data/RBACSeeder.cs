using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Data;

public static class RBACSeeder
{
    public static void SeedRBACData(this ModelBuilder modelBuilder)
    {
        // Seed Modules (based on actual Angular routes)
        var modules = new[]
        {
            new Module
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Dashboard",
                DisplayName = "Dashboard",
                Description = "Main dashboard and overview",
                Icon = "dashboard",
                Route = "/admin/dashboard",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Users",
                DisplayName = "User Management",
                Description = "Manage system users and their access",
                Icon = "user",
                Route = "/admin/users",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Roles",
                DisplayName = "Role Management",
                Description = "Manage user roles and permissions",
                Icon = "safety",
                Route = "/admin/roles",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Modules",
                DisplayName = "Module Management",
                Description = "Manage system modules",
                Icon = "appstore",
                Route = "/admin/modules",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        modelBuilder.Entity<Module>().HasData(modules);

        // Seed Roles
        var adminRoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var userRoleId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var superUserRoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var roles = new[]
        {
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "Administrator with full access to all modules",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Standard user with limited access",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Role
            {
                Id = superUserRoleId,
                Name = "SuperUser",
                Description = "Super administrator with unrestricted access",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        modelBuilder.Entity<Role>().HasData(roles);

        // Seed Admin Role Permissions (full access to all modules)
        var adminPermissions = modules.Select(m => new RoleModulePermission
        {
            Id = Guid.NewGuid(),
            RoleId = adminRoleId,
            ModuleId = m.Id,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true,
            CreatedAt = DateTime.UtcNow
        }).ToArray();

        modelBuilder.Entity<RoleModulePermission>().HasData(adminPermissions);

        // Seed SuperUser Role Permissions (full access to all modules)
        var superUserPermissions = modules.Select(m => new RoleModulePermission
        {
            Id = Guid.NewGuid(),
            RoleId = superUserRoleId,
            ModuleId = m.Id,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true,
            CreatedAt = DateTime.UtcNow
        }).ToArray();

        modelBuilder.Entity<RoleModulePermission>().HasData(superUserPermissions);

        // Seed User Role Permissions (view-only access to Dashboard and Users)
        var userPermissions = new[]
        {
            new RoleModulePermission
            {
                Id = Guid.NewGuid(),
                RoleId = userRoleId,
                ModuleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Dashboard
                CanView = true,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            },
            new RoleModulePermission
            {
                Id = Guid.NewGuid(),
                RoleId = userRoleId,
                ModuleId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Users
                CanView = true,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<RoleModulePermission>().HasData(userPermissions);
    }
}
