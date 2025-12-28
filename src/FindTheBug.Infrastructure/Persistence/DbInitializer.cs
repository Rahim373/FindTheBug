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

            // Seed Diagnostic Tests
            await SeedDiagnosticTestsAsync(context);

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
            new { Name = ModuleConstants.Accounts, DisplayName = "Accounts Management", Description = "Manage expensed, money flow" },
            new { Name = ModuleConstants.Reception, DisplayName = "Receiption Management", Description = "Manage receipts, ticketing, serials" },
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

    private async Task SeedDiagnosticTestsAsync(ApplicationDbContext context)
    {
        // Seed Diagnostic Tests from CSV data
        var diagnosticTestsData = new[]
        {
            // Hematology
            new { TestName = "CBC (Cell Counter)", Category = "Hematology", Price = 400m },
            new { TestName = "HB%", Category = "Hematology", Price = 200m },
            new { TestName = "BT/CT", Category = "Hematology", Price = 600m },
            new { TestName = "ESR", Category = "Hematology", Price = 200m },
            new { TestName = "Blood Grouping", Category = "Hematology", Price = 100m },
            new { TestName = "Platelet Test", Category = "Hematology", Price = 250m },
            new { TestName = "Cross Matching & Screening", Category = "Hematology", Price = 1200m },
            new { TestName = "Total Circuiting Eosinophil Count", Category = "Hematology", Price = 300m },
            
            // Biochemistry
            new { TestName = "RBS With CUS", Category = "Biochemistry", Price = 150m },
            new { TestName = "FBS & 2HABF With CUS", Category = "Biochemistry", Price = 300m },
            new { TestName = "Serum Creatinine", Category = "Biochemistry", Price = 500m },
            new { TestName = "Serum Bilirubin (Total)", Category = "Biochemistry", Price = 400m },
            new { TestName = "Serum Bilirubin (Total+ Direct + Indirect)", Category = "Biochemistry", Price = 900m },
            new { TestName = "Serum Albumin", Category = "Biochemistry", Price = 800m },
            new { TestName = "Serum Urea", Category = "Biochemistry", Price = 500m },
            new { TestName = "S.AST (SGPT)", Category = "Biochemistry", Price = 400m },
            new { TestName = "S.AST (SGOT)", Category = "Biochemistry", Price = 400m },
            new { TestName = "S. ALP (Alkaline phosphatase)", Category = "Biochemistry", Price = 800m },
            new { TestName = "Serum Calcium", Category = "Biochemistry", Price = 800m },
            new { TestName = "Serum Uric Acid", Category = "Biochemistry", Price = 500m },
            new { TestName = "Serum Magnesium", Category = "Biochemistry", Price = 800m },
            new { TestName = "Serum Total Protein", Category = "Biochemistry", Price = 800m },
            new { TestName = "S. Total Cholesterol", Category = "Biochemistry", Price = 300m },
            new { TestName = "S. Triglyceride (TG)", Category = "Biochemistry", Price = 300m },
            new { TestName = "S. HDL", Category = "Biochemistry", Price = 300m },
            new { TestName = "S. LDL", Category = "Biochemistry", Price = 300m },
            new { TestName = "Lipid Profile", Category = "Biochemistry", Price = 800m },
            new { TestName = "Liver Function Test (LFTS)", Category = "Biochemistry", Price = 1400m },
            new { TestName = "OGTT", Category = "Biochemistry", Price = 400m },
            new { TestName = "Urine HCT", Category = "Biochemistry", Price = 300m },
            new { TestName = "Serum Iron", Category = "Biochemistry", Price = 800m },
            new { TestName = "Serum Ferritin", Category = "Biochemistry", Price = 1000m },
            new { TestName = "Serum Electrolyte (Serum/Urine)", Category = "Biochemistry", Price = 1000m },
            
            // Serological
            new { TestName = "ASO", Category = "Serological", Price = 400m },
            new { TestName = "RA", Category = "Serological", Price = 500m },
            new { TestName = "CRP", Category = "Serological", Price = 500m },
            new { TestName = "Dengue IgG/IgM", Category = "Serological", Price = 500m },
            new { TestName = "Dengue NS1", Category = "Serological", Price = 300m },
            new { TestName = "WIDAL", Category = "Serological", Price = 400m },
            new { TestName = "Triple Antigen", Category = "Serological", Price = 800m },
            new { TestName = "MT (Montox Test)", Category = "Serological", Price = 300m },
            new { TestName = "Tuberculosis (TB)", Category = "Serological", Price = 500m },
            new { TestName = "HBSAG (ICT)", Category = "Serological", Price = 400m },
            new { TestName = "Anti HCV (ICT)", Category = "Serological", Price = 800m },
            new { TestName = "MP", Category = "Serological", Price = 400m },
            new { TestName = "VDRL", Category = "Serological", Price = 400m },
            new { TestName = "Anti HIV (1 & 2)", Category = "Serological", Price = 600m },
            new { TestName = "Chikungunya (ICT)", Category = "Serological", Price = 1000m },
            new { TestName = "PT (Prothrombin Time)", Category = "Serological", Price = 1000m },
            new { TestName = "TPHA", Category = "Serological", Price = 400m },
            new { TestName = "H. Pylori", Category = "Serological", Price = 600m },
            new { TestName = "HBsAg (Level)", Category = "Serological", Price = 1200m },
            new { TestName = "HBeAg", Category = "Serological", Price = 1000m },
            
            // Immunological
            new { TestName = "S. TSH", Category = "Immunological", Price = 1000m },
            new { TestName = "S. FT4", Category = "Immunological", Price = 1000m },
            new { TestName = "S. FT3", Category = "Immunological", Price = 1000m },
            new { TestName = "S. T4", Category = "Immunological", Price = 1000m },
            new { TestName = "S. T3", Category = "Immunological", Price = 1000m },
            new { TestName = "S. Prolactin", Category = "Immunological", Price = 1200m },
            new { TestName = "S. Ferritin", Category = "Immunological", Price = 1200m },
            new { TestName = "S. FSH", Category = "Immunological", Price = 1200m },
            new { TestName = "S. LH", Category = "Immunological", Price = 1200m },
            new { TestName = "S. Beta hCG/ b-hCG", Category = "Immunological", Price = 1200m },
            new { TestName = "S. 25 OH Vitamin-D", Category = "Immunological", Price = 3000m },
            new { TestName = "Anti CCP", Category = "Immunological", Price = 3500m },
            new { TestName = "S. IgE (Level)", Category = "Immunological", Price = 1200m },
            new { TestName = "S. Troponin I (Level)", Category = "Immunological", Price = 1200m },
            new { TestName = "S. Testosterone", Category = "Immunological", Price = 1400m },
            new { TestName = "S. HbA1C", Category = "Immunological", Price = 1000m },
            new { TestName = "Troponin I (ICT)", Category = "Immunological", Price = 800m },
            new { TestName = "CRP (Level)", Category = "Immunological", Price = 800m },
            new { TestName = "AMH", Category = "Immunological", Price = 4500m },
            new { TestName = "Semen Analysis", Category = "Immunological", Price = 1000m },
            
            // Urine & Stool
            new { TestName = "Urine PT (HCG)", Category = "Urine & Stool", Price = 100m },
            new { TestName = "Urine R/M/E", Category = "Urine & Stool", Price = 200m },
            new { TestName = "Urine R/M/E (Analyzer)", Category = "Urine & Stool", Price = 400m },
            new { TestName = "Urine Albumin/Protein", Category = "Urine & Stool", Price = 100m },
            new { TestName = "CUS", Category = "Urine & Stool", Price = 100m },
            new { TestName = "Urine PH", Category = "Urine & Stool", Price = 100m },
            new { TestName = "Urine C/S", Category = "Urine & Stool", Price = 1200m },
            new { TestName = "Stool RME", Category = "Urine & Stool", Price = 600m },
            
            // Ultrasonography
            new { TestName = "USG of Pregnancy Profile (P/P)", Category = "Ultrasonography", Price = 800m },
            new { TestName = "USG of Whole Abdomen (W/A)", Category = "Ultrasonography", Price = 800m },
            new { TestName = "USG of Lower Abdomen (L/A)", Category = "Ultrasonography", Price = 800m },
            new { TestName = "USG of Both Brest", Category = "Ultrasonography", Price = 1200m },
            new { TestName = "USG of TVS", Category = "Ultrasonography", Price = 2000m },
            new { TestName = "USG of KUB", Category = "Ultrasonography", Price = 800m },
            new { TestName = "USG of HBS", Category = "Ultrasonography", Price = 800m },
            new { TestName = "USG of Scrotum & Testis", Category = "Ultrasonography", Price = 1000m },
            new { TestName = "USG of Thyroid", Category = "Ultrasonography", Price = 1600m },
            
            // Electrocardiography
            new { TestName = "ECG (6 Channel)", Category = "Electrocardiography", Price = 300m },
            new { TestName = "ECG (12 Channel)", Category = "Electrocardiography", Price = 400m },
            new { TestName = "Echo", Category = "Electrocardiography", Price = 2500m },
            
            // Radiology & Imaging
            new { TestName = "X-ray Chest (P/A, A/P) View", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray Lumber Spine (B/V)", Category = "Radiology & Imaging", Price = 600m },
            new { TestName = "X-ray of KUB", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of (Hand, Leg, Foot) B/V", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of PNS OM View", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of (C/S, D/L)", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of Abdomen", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of HIP Joint B/V", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of Shoulder Joint B/V", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of Knee Joint B/V", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray of ELBOW Joint B/V", Category = "Radiology & Imaging", Price = 500m },
            new { TestName = "X-ray Online Report", Category = "Radiology & Imaging", Price = 100m }
        };

        foreach (var testData in diagnosticTestsData)
        {
            if (!await context.DiagnosticTests.AnyAsync(dt => dt.TestName == testData.TestName && dt.Category == testData.Category))
            {
                var diagnosticTest = new DiagnosticTest
                {
                    TestName = testData.TestName,
                    Category = testData.Category,
                    Price = testData.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                };
                await context.DiagnosticTests.AddAsync(diagnosticTest);
            }
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Diagnostic tests seeded successfully");
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