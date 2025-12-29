using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for TestEntry entity
/// </summary>
public class ReceiptTestMapping : IMapping<ReceiptTest>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReceiptTest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Total)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.DiscountPercentage)
                .HasPrecision(5, 2)
                .HasColumnType("numeric(5,2)");

            entity.Property(e => e.Amount)
                .HasPrecision(5, 2)
                .HasColumnType("numeric(5,2)");

            entity.HasIndex(e => e.LabReceiptId);
            entity.HasIndex(e => e.DiagnosticTestId);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.TestEntries)
                .HasForeignKey(e => e.LabReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DiagnosticTest)
                .WithMany(t => t.TestEntries)
                .HasForeignKey(e => e.DiagnosticTestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.TestResults)
                .WithOne(tr => tr.TestEntry)
                .HasForeignKey(tr => tr.TestEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.InvoiceItems)
                .WithOne(ii => ii.TestEntry)
                .HasForeignKey(ii => ii.TestEntryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.ToTable("ReceiptTests");
        });
    }
}
