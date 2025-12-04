using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string? Email { get; set; }
    public required string PasswordHash { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NIDNumber { get; set; }
    public string Roles { get; set; } = "User"; // Comma-separated roles: "Admin,User"
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedOutUntil { get; set; }

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
