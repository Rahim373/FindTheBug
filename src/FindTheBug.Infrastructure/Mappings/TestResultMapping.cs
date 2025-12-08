using FindTheBug.Infrastructure.Common;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for TestResult entity
/// </summary>
public class TestResultMapping : IMapping<TestResult>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.TestEntryId);
            entity.HasIndex(e => e.TestParameterId);
            
            entity.Property(e => e.TestEntryId)
                .IsRequired()
                .HasColumnType("uuid");
                
            entity.Property(e => e.TestParameterId)
                .IsRequired()
                .HasColumnType("uuid");
                
            entity.Property(e => e.ResultValue)
                .IsRequired()
                .HasColumnType("text");
                
            entity.Property(e => e.IsAbnormal)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);
                
            entity.Property(e => e.ResultDate)
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.VerifiedBy)
                .HasColumnType("text");
                
            entity.Property(e => e.VerifiedDate)
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
            entity.HasOne(e => e.TestEntry)
                .WithMany(te => te.TestResults)
                .HasForeignKey(e => e.TestEntryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.TestParameter)
                .WithMany(tp => tp.TestResults)
                .HasForeignKey(e => e.TestParameterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("TestResults");
        });
    }
}
