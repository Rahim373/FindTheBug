using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Patient entity
/// </summary>
public class PatientMapping : IMapping<LabReceipt>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LabReceipt>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.InvoiceNumber).IsUnique();

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Gender)
                .HasColumnType("text");

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Address)
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            entity.Property(e => e.IsAgeYear)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            entity.Property(e => e.Total)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.Due)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.Discount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.Balace)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            // Navigation properties configuration
            entity.HasMany(e => e.TestEntries)
                .WithOne(t => t.Patient)
                .HasForeignKey(t => t.LabReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Invoices)
                .WithOne(i => i.Patient)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.TestEntries)
                .WithOne(t => t.Patient)
                .HasForeignKey(t => t.LabReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("LabReceipts");
        });
    }
}
