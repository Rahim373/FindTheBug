using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for RefreshToken entity
/// </summary>
public class RefreshTokenMapping : IMapping<RefreshToken>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.UserId);
            
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnType("uuid");
                
            entity.Property(e => e.Token)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.CreatedByIp)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.RevokedAt)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.RevokedByIp)
                .HasColumnType("text");
                
            entity.Property(e => e.ReplacedByToken)
                .HasColumnType("text");
                
            entity.Property(e => e.ReasonRevoked)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("RefreshTokens");
        });
    }
}
