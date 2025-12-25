using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class DoctorSpeciality : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<DoctorSpecialityMap> DoctorMappings { get; set; } = new List<DoctorSpecialityMap>();
}
