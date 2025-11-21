using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class User : BaseAuditableEntity, ITenantEntity
{
    public required string TenantId { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Roles { get; set; } = "User"; // Comma-separated roles: "Admin,User"
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedOutUntil { get; set; }

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
