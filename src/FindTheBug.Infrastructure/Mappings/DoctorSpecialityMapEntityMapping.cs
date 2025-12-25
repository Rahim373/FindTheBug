using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

public class DoctorSpecialityMapEntityMapping : IMapping<DoctorSpecialityMap>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        // Configure the many-to-many junction table
        modelBuilder.Entity<DoctorSpecialityMap>(entity =>
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
