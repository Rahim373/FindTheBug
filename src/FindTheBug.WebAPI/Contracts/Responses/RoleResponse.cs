namespace FindTheBug.WebAPI.Contracts.Responses;

public class RoleResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
