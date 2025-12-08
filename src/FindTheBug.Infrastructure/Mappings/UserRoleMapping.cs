using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for UserRole entity
/// </summary>
public class UserRoleMapping : IMapping<UserRole>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            entity.HasIndex(e => e.RoleId);

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.AssignedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.AssignedBy)
                .HasColumnType("uuid");

            // Navigation properties configuration
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("UserRoles");
        });
    }
}
