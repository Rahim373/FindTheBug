using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for TestParameter entity
/// </summary>
public class TestParameterMapping : IMapping<TestParameter>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestParameter>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.DiagnosticTestId);

            entity.Property(e => e.DiagnosticTestId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.ParameterName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Unit)
                .HasColumnType("text");

            entity.Property(e => e.ReferenceRangeMin)
                .HasPrecision(18, 4)
                .HasColumnType("numeric(18,4)");

            entity.Property(e => e.ReferenceRangeMax)
                .HasPrecision(18, 4)
                .HasColumnType("numeric(18,4)");

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.DisplayOrder)
                .IsRequired()
                .HasColumnType("integer");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasOne(e => e.DiagnosticTest)
                .WithMany(dt => dt.Parameters)
                .HasForeignKey(e => e.DiagnosticTestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.TestResults)
                .WithOne(tr => tr.TestParameter)
                .HasForeignKey(tr => tr.TestParameterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("TestParameters");
        });
    }
}
