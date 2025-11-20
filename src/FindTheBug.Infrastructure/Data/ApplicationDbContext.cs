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
        if (_tenantContext?.TenantId != null)
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

        // Apply configurations here
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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

