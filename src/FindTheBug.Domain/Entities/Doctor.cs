using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Degree { get; set; }
    public string? Office { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<DoctorSpecialityMap> DoctorSpecialities { get; set; } = new List<DoctorSpecialityMap>();
    public ICollection<LabReceipt> LabReceipts { get; set; } = new List<LabReceipt>();
}
