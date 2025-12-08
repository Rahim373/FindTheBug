using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Patient entity
/// </summary>
public class PatientMapping : IMapping<Patient>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.MobileNumber).IsUnique();
            entity.HasIndex(e => e.PatientCode).IsUnique();

            entity.Property(e => e.PatientCode)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.DateOfBirth)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.Gender)
                .HasColumnType("text");

            entity.Property(e => e.MobileNumber)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Email)
                .HasColumnType("text");

            entity.Property(e => e.Address)
                .HasColumnType("text");

            entity.Property(e => e.City)
                .HasColumnType("text");

            entity.Property(e => e.PostalCode)
                .HasColumnType("text");

            entity.Property(e => e.EmergencyContact)
                .HasColumnType("text");

            entity.Property(e => e.EmergencyContactNumber)
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
            entity.HasMany(e => e.TestEntries)
                .WithOne(t => t.Patient)
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Invoices)
                .WithOne(i => i.Patient)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("Patients");
        });
    }
}
