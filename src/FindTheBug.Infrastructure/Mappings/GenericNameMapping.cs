using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for GenericName entity
/// </summary>
public class GenericNameMapping : IMapping<GenericName>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GenericName>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Description)
                .HasColumnType("text");

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
            entity.HasMany(e => e.Drugs)
                .WithOne(d => d.GenericName)
                .HasForeignKey(d => d.GenericNameId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("GenericNames");
        });
    }
}
