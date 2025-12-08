using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Invoice entity
/// </summary>
public class InvoiceMapping : IMapping<Invoice>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.PatientId);
            
            entity.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnType("uuid");
                
            entity.Property(e => e.InvoiceNumber)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.InvoiceDate)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");
                
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)")
                .HasDefaultValue(0);
                
            entity.Property(e => e.TaxAmount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)")
                .HasDefaultValue(0);
                
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("text");
                
            entity.Property(e => e.PaymentDate)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.Notes)
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
                .WithMany(p => p.Invoices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.InvoiceItems)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("Invoices");
        });
    }
}
