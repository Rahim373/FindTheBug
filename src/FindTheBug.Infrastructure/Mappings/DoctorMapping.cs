using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Doctor entity
/// </summary>
public class DoctorMapping : IMapping<Doctor>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.PhoneNumber).IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Degree)
                .HasColumnType("text");

            entity.Property(e => e.Office)
                .HasColumnType("text");

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
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
            entity.HasMany(e => e.DoctorSpecialities)
                .WithOne(dsm => dsm.Doctor)
                .HasForeignKey(dsm => dsm.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.LabReceipts)
                .WithOne(dsm => dsm.ReferredBy)
                .HasForeignKey(dsm => dsm.ReferredByDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("Doctors");
        });
    }
}
