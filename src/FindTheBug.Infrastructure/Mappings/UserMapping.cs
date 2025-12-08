using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserMapping : IMapping<User>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .HasColumnType("text");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Phone)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.NIDNumber)
                .HasColumnType("text");

            entity.Property(e => e.IsActive)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            entity.Property(e => e.AllowUserLogin)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.LastLoginIp)
                .HasColumnType("text");

            entity.Property(e => e.FailedLoginAttempts)
                .HasColumnType("integer")
                .HasDefaultValue(0);

            entity.Property(e => e.LockedOutUntil)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasMany(e => e.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("Users");
        });
    }
}
