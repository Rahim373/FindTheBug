using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext? _tenantContext;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext) 
        : base(options)
    {
        _tenantContext = tenantContext;
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
        
        ConfigureLabManagementEntities(modelBuilder);
        ConfigureRBACEntities(modelBuilder);
    }

    private static void ConfigureLabManagementEntities(ModelBuilder modelBuilder)
    {
        // DiagnosticTest configuration
        modelBuilder.Entity<DiagnosticTest>(entity =>
        {
            entity.HasIndex(e => new { e.TestCode }).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => new { e.MobileNumber }).IsUnique();
            entity.HasIndex(e => new { e.PatientCode }).IsUnique();
        });

        // TestEntry configuration
        modelBuilder.Entity<TestEntry>(entity =>
        {
            entity.HasIndex(e => new { e.EntryNumber }).IsUnique();
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.TestEntries)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.DiagnosticTest)
                .WithMany(t => t.TestEntries)
                .HasForeignKey(e => e.DiagnosticTestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TestParameter configuration
        modelBuilder.Entity<TestParameter>(entity =>
        {
            entity.HasOne(e => e.DiagnosticTest)
                .WithMany(t => t.Parameters)
                .HasForeignKey(e => e.DiagnosticTestId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(e => e.ReferenceRangeMin).HasPrecision(18, 4);
            entity.Property(e => e.ReferenceRangeMax).HasPrecision(18, 4);
        });

        // TestResult configuration
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasOne(e => e.TestEntry)
                .WithMany(t => t.TestResults)
                .HasForeignKey(e => e.TestEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.TestParameter)
                .WithMany(p => p.TestResults)
                .HasForeignKey(e => e.TestParameterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => new { e.InvoiceNumber }).IsUnique();
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
        });

        // InvoiceItem configuration
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.TestEntry)
                .WithMany(t => t.InvoiceItems)
                .HasForeignKey(e => e.TestEntryId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
        });
    }

    private static void ConfigureRBACEntities(ModelBuilder modelBuilder)
    {
        // Module configuration
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Route).HasMaxLength(200);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // UserRole configuration (many-to-many)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RoleModulePermission configuration
        modelBuilder.Entity<RoleModulePermission>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.ModuleId }).IsUnique();
            
            entity.HasOne(rmp => rmp.Role)
                .WithMany(r => r.RoleModulePermissions)
                .HasForeignKey(rmp => rmp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(rmp => rmp.Module)
                .WithMany(m => m.RoleModulePermissions)
                .HasForeignKey(rmp => rmp.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

