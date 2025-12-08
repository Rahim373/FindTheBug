using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for InvoiceItem entity
/// </summary>
public class InvoiceItemMapping : IMapping<InvoiceItem>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.TestEntryId);

            entity.Property(e => e.InvoiceId)
                .IsRequired()
                .HasColumnType("uuid");

            entity.Property(e => e.TestEntryId)
                .HasColumnType("uuid");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasColumnType("integer");

            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("numeric")
                .HasDefaultValue(0);

            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            // Navigation properties configuration
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TestEntry)
                .WithMany(te => te.InvoiceItems)
                .HasForeignKey(e => e.TestEntryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.ToTable("InvoiceItems");
        });
    }
}
