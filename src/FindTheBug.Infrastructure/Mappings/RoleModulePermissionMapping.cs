using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for RoleModulePermission entity
/// </summary>
public class RoleModulePermissionMapping : IMapping<RoleModulePermission>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleModulePermission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.RoleId, e.ModuleId }).IsUnique();
            entity.HasIndex(e => e.ModuleId);
            entity.HasIndex(e => e.RoleId);

            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.ModuleId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.CanView)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            entity.Property(e => e.CanCreate)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            entity.Property(e => e.CanEdit)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            entity.Property(e => e.CanDelete)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            // Navigation properties configuration
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RoleModulePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Module)
                .WithMany(m => m.RoleModulePermissions)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("RoleModulePermissions");
        });
    }
}
