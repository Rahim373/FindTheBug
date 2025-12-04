using System.ComponentModel.DataAnnotations;

namespace FindTheBug.WebAPI.Contracts.Requests;

public class CreateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    [Required]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? NIDNumber { get; set; }

    public string? Roles { get; set; }

    public bool IsActive { get; set; } = true;
}
