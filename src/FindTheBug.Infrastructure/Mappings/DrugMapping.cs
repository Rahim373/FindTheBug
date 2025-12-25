using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Drug entity
/// </summary>
public class DrugMapping : IMapping<Drug>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.GenericNameId)
                .IsRequired();

            entity.Property(e => e.BrandId)
                .IsRequired();

            entity.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            entity.Property(e => e.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.PhotoPath)
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
            entity.HasOne(e => e.GenericName)
                .WithMany(gn => gn.Drugs)
                .HasForeignKey(e => e.GenericNameId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Brand)
                .WithMany(b => b.Drugs)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("Drugs");
        });
    }
}
