using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for DoctorSpeciality entity and junction table
/// </summary>
public class DoctorSpecialityEntityMapping : IMapping<DoctorSpeciality>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorSpeciality>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Description)
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
            entity.HasMany(e => e.DoctorMappings)
                .WithOne(dsm => dsm.DoctorSpeciality)
                .HasForeignKey(dsm => dsm.DoctorSpecialityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("DoctorSpecialities");
        });

        // Configure the many-to-many junction table
        modelBuilder.Entity<DoctorSpecialityMapping>(entity =>
        {
            entity.HasKey(e => new { e.DoctorId, e.DoctorSpecialityId });

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            // Navigation properties
            entity.HasOne(dsm => dsm.Doctor)
                .WithMany(d => d.DoctorSpecialities)
                .HasForeignKey(dsm => dsm.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dsm => dsm.DoctorSpeciality)
                .WithMany(ds => ds.DoctorMappings)
                .HasForeignKey(dsm => dsm.DoctorSpecialityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("DoctorSpecialityMappings");
        });
    }
}
