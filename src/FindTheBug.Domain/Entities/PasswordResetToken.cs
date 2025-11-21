using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
    public string? IpAddress { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsUsed => UsedAt is not null;
    public bool IsValid => !IsExpired && !IsUsed;
}
