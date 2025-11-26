using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Patient : BaseAuditableEntity
{
    public string PatientCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public required string MobileNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<TestEntry> TestEntries { get; set; } = new List<TestEntry>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
