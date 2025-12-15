using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class DoctorSpecialityMapping : BaseEntity
{
    public Guid DoctorId { get; set; }
    public Guid DoctorSpecialityId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Doctor Doctor { get; set; } = null!;
    public DoctorSpeciality DoctorSpeciality { get; set; } = null!;
}
