using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FindTheBug.Infrastructure.Persistence;

public class DbInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IConfiguration _configuration;

    public DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations
            if (context.Database.IsNpgsql() && context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed RBAC data (modules, roles, permissions)
            await SeedRBACDataAsync(context);

            // Seed SuperUser
            await SeedSuperUserAsync(context, scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedRBACDataAsync(ApplicationDbContext context)
    {
        // Seed Modules
        var moduleDefinitions = new[]
        {
            new { Name = "Dashboard", DisplayName = "Dashboard", Description = "Main dashboard and overview" },
            new { Name = "Users", DisplayName = "User Management", Description = "Manage system users and their access" },
            new { Name = "Roles", DisplayName = "Role Management", Description = "Manage user roles and permissions" },
            new { Name = "Modules", DisplayName = "Module Management", Description = "Manage system modules" }
        };

        foreach (var moduleDef in moduleDefinitions)
        {
            if (!await context.Modules.AnyAsync(m => m.Name == moduleDef.Name))
            {
                var module = new Module
                {
                    Name = moduleDef.Name,
                    DisplayName = moduleDef.DisplayName,
                    Description = moduleDef.Description,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await context.Modules.AddAsync(module);
            }
        }
        await context.SaveChangesAsync();
        _logger.LogInformation("Modules seeded successfully");

        // Seed Roles
        var roleDefinitions = new[]
        {
            new { Name = "Admin", Description = "Administrator with full access to all modules", IsSystemRole = true },
            new { Name = "User", Description = "Standard user with limited access", IsSystemRole = true },
            new { Name = "SuperUser", Description = "Super administrator with unrestricted access", IsSystemRole = true }
        };

        foreach (var roleDef in roleDefinitions)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == roleDef.Name))
            {
                var role = new Role
                {
                    Name = roleDef.Name,
                    Description = roleDef.Description,
                    IsSystemRole = roleDef.IsSystemRole,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await context.Roles.AddAsync(role);
            }
        }
        await context.SaveChangesAsync();
        _logger.LogInformation("Roles seeded successfully");

        // Seed Role Permissions
        await SeedRolePermissionsAsync(context);
    }

    private async Task SeedRolePermissionsAsync(ApplicationDbContext context)
    {
        var modules = await context.Modules.ToListAsync();
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        var superUserRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperUser");

        if (adminRole == null || userRole == null || superUserRole == null)
        {
            _logger.LogWarning("One or more roles not found. Skipping permission seeding.");
            return;
        }

        // Admin permissions (full access to all modules)
        foreach (var module in modules)
        {
            if (!await context.RoleModulePermissions.AnyAsync(p => p.RoleId == adminRole.Id && p.ModuleId == module.Id))
            {
                await context.RoleModulePermissions.AddAsync(new RoleModulePermission
                {
                    RoleId = adminRole.Id,
                    ModuleId = module.Id,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanDelete = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // SuperUser permissions (full access to all modules)
        foreach (var module in modules)
        {
            if (!await context.RoleModulePermissions.AnyAsync(p => p.RoleId == superUserRole.Id && p.ModuleId == module.Id))
            {
                await context.RoleModulePermissions.AddAsync(new RoleModulePermission
                {
                    RoleId = superUserRole.Id,
                    ModuleId = module.Id,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanDelete = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // User permissions (view-only access to Dashboard and Users)
        var dashboardModule = modules.FirstOrDefault(m => m.Name == "Dashboard");
        var usersModule = modules.FirstOrDefault(m => m.Name == "Users");

        if (dashboardModule != null && !await context.RoleModulePermissions.AnyAsync(p => p.RoleId == userRole.Id && p.ModuleId == dashboardModule.Id))
        {
            await context.RoleModulePermissions.AddAsync(new RoleModulePermission
            {
                RoleId = userRole.Id,
                ModuleId = dashboardModule.Id,
                CanView = true,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (usersModule != null && !await context.RoleModulePermissions.AnyAsync(p => p.RoleId == userRole.Id && p.ModuleId == usersModule.Id))
        {
            await context.RoleModulePermissions.AddAsync(new RoleModulePermission
            {
                RoleId = userRole.Id,
                ModuleId = usersModule.Id,
                CanView = true,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Role permissions seeded successfully");
    }

    private async Task SeedSuperUserAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        var superUserConfig = _configuration.GetSection("SuperUser");
        var email = superUserConfig["Email"];
        
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("SuperUser email not configured. Skipping seeding.");
            return;
        }

        if (await context.Users.AnyAsync(u => u.Email == email))
        {
            return; // User already exists
        }

        var password = superUserConfig["Password"];
        var firstName = superUserConfig["FirstName"] ?? "Super";
        var lastName = superUserConfig["LastName"] ?? "User";
        var phoneNumber = superUserConfig["PhoneNumber"] ?? "01734014433";

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogError("SuperUser password not configured.");
            return;
        }

        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        var passwordHash = passwordHasher.HashPassword(password);

        var user = new User
        {
            Phone = phoneNumber,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "System",
            AllowUserLogin = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Assign SuperUser role
        var superUserRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperUser");
        if (superUserRole != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = superUserRole.Id,
                AssignedAt = DateTime.UtcNow
            };
            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
            _logger.LogInformation("SuperUser seeded successfully with SuperUser role assigned.");
        }
        else
        {
            _logger.LogWarning("SuperUser role not found in database. User created without role assignment.");
        }
    }
}
