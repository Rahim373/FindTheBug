using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Role entity
/// </summary>
public class RoleMapping : IMapping<Role>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Name).IsUnique();
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");
                
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");
                
            entity.Property(e => e.IsSystemRole)
                .HasColumnType("boolean")
                .HasDefaultValue(false);
                
            entity.Property(e => e.IsActive)
                .HasColumnType("boolean")
                .HasDefaultValue(true);
                
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");
                
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasMany(e => e.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.RoleModulePermissions)
                .WithOne(rmp => rmp.Role)
                .HasForeignKey(rmp => rmp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("Roles");
        });
    }
}
