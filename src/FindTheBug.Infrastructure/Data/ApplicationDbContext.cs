using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using FindTheBug.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Lab Management DbSets
    public DbSet<DiagnosticTest> DiagnosticTests => Set<DiagnosticTest>();
    public DbSet<TestParameter> TestParameters => Set<TestParameter>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TestEntry> TestEntries => Set<TestEntry>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Authentication DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // RBAC DbSets
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RoleModulePermission> RoleModulePermissions => Set<RoleModulePermission>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Add audit information before saving
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseAuditableEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = "System"; // Replace with actual user
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "System"; // Replace with actual user
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity mappings dynamically using reflection
        ApplyMappings(modelBuilder);
    }

    private void ApplyMappings(ModelBuilder modelBuilder)
    {
        var mappingTypes = typeof(DiagnosticTestMapping).Assembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMapping<>)))
            .ToList();

        foreach (var mappingType in mappingTypes)
        {
            var mappingInstance = Activator.CreateInstance(mappingType);
            var configureMethod = mappingType.GetMethod("Configure");
            configureMethod?.Invoke(mappingInstance, new object[] { modelBuilder });
        }
    }
}
