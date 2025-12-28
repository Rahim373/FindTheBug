using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for DiagnosticTest entity
/// </summary>
public class DiagnosticTestMapping : IMapping<DiagnosticTest>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiagnosticTest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TestName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Description)
                .HasColumnType("text");

            entity.Property(e => e.Category)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.Duration)
                .HasColumnType("text");

            entity.Property(e => e.RequiresFasting)
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

            entity.ToTable("DiagnosticTests");
        });
    }
}
