using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Product entity
/// </summary>
public class ProductMapping : IMapping<Product>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnType("text");

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

            entity.ToTable("Products");
        });
    }
}
