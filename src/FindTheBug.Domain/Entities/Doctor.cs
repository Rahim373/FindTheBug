using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Degree { get; set; }
    public string? Office { get; set; }
    public required string PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<DoctorSpecialityMapping> DoctorSpecialities { get; set; } = new List<DoctorSpecialityMapping>();
}
