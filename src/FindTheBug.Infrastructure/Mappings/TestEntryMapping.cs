using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for TestEntry entity
/// </summary>
public class TestEntryMapping : IMapping<TestEntry>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.EntryNumber).IsUnique();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DiagnosticTestId);
            
            entity.Property(e => e.EntryNumber)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.EntryDate)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.SampleCollectionDate)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.Priority)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.ReferredBy)
                .HasColumnType("text");
                
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
                .WithMany(p => p.TestEntries)
                .HasForeignKey(e => e.PatientId)
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

            entity.ToTable("TestEntries");
        });
    }
}
