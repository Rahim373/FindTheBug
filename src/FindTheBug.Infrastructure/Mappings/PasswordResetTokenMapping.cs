using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for PasswordResetToken entity
/// </summary>
public class PasswordResetTokenMapping : IMapping<PasswordResetToken>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.Token)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.UsedAt)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.IpAddress)
                .HasColumnType("text");

            entity.ToTable("PasswordResetTokens");
        });
    }
}
