using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Contracts;
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

            // Apply migrations - this will create database if it doesn't exist
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            // Seed RBAC data (modules, roles, permissions)
            await SeedRBACDataAsync(context);

            // Seed Doctor Specialities
            await SeedDoctorSpecialitiesAsync(context);

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
            new { Name = ModuleConstants.UserManagement, DisplayName = "User Management", Description = "Manage system users and their access" },
            new { Name = ModuleConstants.Dispensary, DisplayName = "Dispensary Management", Description = "Manage dispensary, product and sales" },
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
            new { Name = RoleConstants.SuperUser, Description = "Super administrator with unrestricted access", IsSystemRole = true }
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
        var superUserRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.SuperUser);

        if (superUserRole is null)
        {
            _logger.LogWarning("Superuser role not found. Skipping permission seeding.");
            return;
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

        await context.SaveChangesAsync();
        _logger.LogInformation("Role permissions seeded successfully");
    }

    private async Task SeedDoctorSpecialitiesAsync(ApplicationDbContext context)
    {
        // Seed Doctor Specialities from CSV data
        var specialityData = new[]
        {
            new { Category = "Primary Care & General", Name = "General Practice (MBBS)" },
            new { Category = "Primary Care & General", Name = "Internal Medicine" },
            new { Category = "Primary Care & General", Name = "Preventive & Social Medicine" },
            new { Category = "Medicine Subspecialties", Name = "Cardiology" },
            new { Category = "Medicine Subspecialties", Name = "Endocrinology" },
            new { Category = "Medicine Subspecialties", Name = "Gastroenterology" },
            new { Category = "Medicine Subspecialties", Name = "Hepatology" },
            new { Category = "Medicine Subspecialties", Name = "Nephrology" },
            new { Category = "Medicine Subspecialties", Name = "Pulmonology (Respiratory Medicine)" },
            new { Category = "Medicine Subspecialties", Name = "Neurology" },
            new { Category = "Medicine Subspecialties", Name = "Rheumatology" },
            new { Category = "Medicine Subspecialties", Name = "Hematology" },
            new { Category = "Medicine Subspecialties", Name = "Medical Oncology" },
            new { Category = "Medicine Subspecialties", Name = "Infectious Diseases" },
            new { Category = "Surgical Specialties", Name = "General Surgery" },
            new { Category = "Surgical Specialties", Name = "Orthopedic Surgery" },
            new { Category = "Surgical Specialties", Name = "Neurosurgery" },
            new { Category = "Surgical Specialties", Name = "Cardiothoracic Surgery" },
            new { Category = "Surgical Specialties", Name = "Pediatric Surgery" },
            new { Category = "Surgical Specialties", Name = "Plastic & Reconstructive Surgery" },
            new { Category = "Surgical Specialties", Name = "Vascular Surgery" },
            new { Category = "Surgical Specialties", Name = "Urology" },
            new { Category = "Women's Health", Name = "Obstetrics & Gynecology" },
            new { Category = "Women's Health", Name = "Gynecologic Oncology" },
            new { Category = "Child Health", Name = "Pediatrics" },
            new { Category = "Child Health", Name = "Neonatology" },
            new { Category = "ENT & Eye", Name = "Otolaryngology (ENT)" },
            new { Category = "ENT & Eye", Name = "Ophthalmology" },
            new { Category = "Mental Health", Name = "Psychiatry" },
            new { Category = "Emergency & Critical Care", Name = "Anesthesiology" },
            new { Category = "Emergency & Critical Care", Name = "Critical Care Medicine" },
            new { Category = "Diagnostics & Imaging", Name = "Radiology & Imaging" },
            new { Category = "Diagnostics & Imaging", Name = "Pathology" },
            new { Category = "Diagnostics & Imaging", Name = "Histopathology" },
            new { Category = "Diagnostics & Imaging", Name = "Microbiology" },
            new { Category = "Diagnostics & Imaging", Name = "Biochemistry" },
            new { Category = "Diagnostics & Imaging", Name = "Hematology (Laboratory)" },
            new { Category = "Public Health", Name = "Community Medicine" },
            new { Category = "Dermatology", Name = "Dermatology & Venereology" },
            new { Category = "Other Fields", Name = "Nuclear Medicine" },
            new { Category = "Other Fields", Name = "Forensic Medicine" },
            new { Category = "Other Fields", Name = "Transfusion Medicine" }
        };

        foreach (var speciality in specialityData)
        {
            if (!await context.DoctorSpecialities.AnyAsync(ds => ds.Name == speciality.Name))
            {
                var doctorSpeciality = new DoctorSpeciality
                {
                    Name = speciality.Name,
                    Description = speciality.Category,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await context.DoctorSpecialities.AddAsync(doctorSpeciality);
            }
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Doctor specialities seeded successfully");
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
