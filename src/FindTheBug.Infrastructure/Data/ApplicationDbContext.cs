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

    public DbSet<SampleEntity> SampleEntities => Set<SampleEntity>();
    
    // Lab Management DbSets
    public DbSet<DiagnosticTest> DiagnosticTests => Set<DiagnosticTest>();
    public DbSet<TestParameter> TestParameters => Set<TestParameter>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TestEntry> TestEntries => Set<TestEntry>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

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

        // Set TenantId for tenant-scoped entities
        if (_tenantContext?.TenantId is not null)
        {
            var tenantEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity);

            foreach (var entry in tenantEntries)
            {
                var tenantEntity = (ITenantEntity)entry.Entity;
                if (string.IsNullOrEmpty(tenantEntity.TenantId))
                {
                    tenantEntity.TenantId = _tenantContext.TenantId;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply global query filter for multi-tenancy
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);
                
                method?.Invoke(null, new object[] { modelBuilder, this });
            }
        }

        // Configure entity relationships and indexes
        ConfigureLabManagementEntities(modelBuilder);
    }

    private static void ConfigureLabManagementEntities(ModelBuilder modelBuilder)
    {
        // DiagnosticTest configuration
        modelBuilder.Entity<DiagnosticTest>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.TestCode }).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.MobileNumber }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.PatientCode }).IsUnique();
        });

        // TestEntry configuration
        modelBuilder.Entity<TestEntry>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.EntryNumber }).IsUnique();
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
            entity.HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
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

    private static void SetGlobalQueryFilter<TEntity>(ModelBuilder modelBuilder, ApplicationDbContext context) 
        where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => 
            context._tenantContext == null || 
            context._tenantContext.TenantId == null || 
            e.TenantId == context._tenantContext.TenantId);
    }
}

