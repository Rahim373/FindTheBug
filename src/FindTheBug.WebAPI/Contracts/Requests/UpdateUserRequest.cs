using System.ComponentModel.DataAnnotations;

namespace FindTheBug.WebAPI.Contracts.Requests;

public class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Required]
    [Phone]
    public required string Phone { get; set; }

    [MaxLength(50)]
    public string? NIDNumber { get; set; }

    public string? Roles { get; set; }

    public bool IsActive { get; set; }

    public bool AllowUserLogin { get; set; }

    [MinLength(6)]
    public string? Password { get; set; }
}
