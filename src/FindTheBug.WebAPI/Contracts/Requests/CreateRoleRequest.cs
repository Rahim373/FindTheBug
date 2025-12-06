using System.ComponentModel.DataAnnotations;

namespace FindTheBug.WebAPI.Contracts.Requests;

public class CreateRoleRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
