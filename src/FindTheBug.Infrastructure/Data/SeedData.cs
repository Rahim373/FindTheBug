using FindTheBug.Domain.Entities;

namespace FindTheBug.Infrastructure.Data;

public static class SeedData
{
    public static List<Module> GetDefaultModules()
    {
        return new List<Module>
        {
            new Module
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Dashboard",
                DisplayName = "Dashboard",
                Description = "Main dashboard view",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Users",
                DisplayName = "User Management",
                Description = "Manage system users",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Roles",
                DisplayName = "Role Management",
                Description = "Manage system roles and permissions",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Patients",
                DisplayName = "Patient Management",
                Description = "Manage patient records",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "DiagnosticTests",
                DisplayName = "Diagnostic Tests",
                Description = "Manage diagnostic tests",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "TestEntries",
                DisplayName = "Test Entries",
                Description = "Manage test entries",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "Invoices",
                DisplayName = "Invoice Management",
                Description = "Manage invoices and billing",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
}
