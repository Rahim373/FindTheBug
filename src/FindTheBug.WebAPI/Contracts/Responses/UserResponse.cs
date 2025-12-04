namespace FindTheBug.WebAPI.Contracts.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NIDNumber { get; set; }
    public string Roles { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool AllowUserLogin { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
