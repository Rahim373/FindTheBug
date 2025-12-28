using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Desktop.Reception.Services.CloudSync.Mappers;

/// <summary>
/// Mappers for converting DTOs to domain entities
/// </summary>
public static class SyncMappers
{
    /// <summary>
    /// Maps UserDto to User entity
    /// </summary>
    public static User ToEntity(this UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            PasswordHash = string.Empty, // Required field, not synced from cloud
            Phone = string.Empty // Required field, not synced from cloud
        };
    }

    /// <summary>
    /// Updates an existing User entity from UserDto
    /// </summary>
    public static void UpdateEntity(this UserDto dto, User entity)
    {
        entity.Email = dto.Email;
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = dto.UpdatedAt;
    }

    /// <summary>
    /// Maps RoleDto to Role entity
    /// </summary>
    public static Role ToEntity(this RoleDto dto)
    {
        return new Role
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    /// <summary>
    /// Updates an existing Role entity from RoleDto
    /// </summary>
    public static void UpdateEntity(this RoleDto dto, Role entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = dto.UpdatedAt;
    }

    /// <summary>
    /// Maps UserRoleDto to UserRole entity
    /// </summary>
    public static UserRole ToEntity(this UserRoleDto dto)
    {
        return new UserRole
        {
            Id = dto.Id,
            UserId = dto.UserId,
            RoleId = dto.RoleId,
            AssignedAt = dto.CreatedAt // Map CreatedAt to AssignedAt
        };
    }

    /// <summary>
    /// Updates an existing UserRole entity from UserRoleDto
    /// </summary>
    public static void UpdateEntity(this UserRoleDto dto, UserRole entity)
    {
        entity.UserId = dto.UserId;
        entity.RoleId = dto.RoleId;
        entity.AssignedAt = dto.CreatedAt; // Map CreatedAt to AssignedAt
    }

    /// <summary>
    /// Maps ModuleDto to Module entity
    /// </summary>
    public static Module ToEntity(this ModuleDto dto)
    {
        return new Module
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            DisplayName = dto.Key, // Map Key to DisplayName
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    /// <summary>
    /// Updates an existing Module entity from ModuleDto
    /// </summary>
    public static void UpdateEntity(this ModuleDto dto, Module entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.DisplayName = dto.Key; // Map Key to DisplayName
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = dto.UpdatedAt;
    }

    /// <summary>
    /// Maps RoleModulePermissionDto to RoleModulePermission entity
    /// </summary>
    public static RoleModulePermission ToEntity(this RoleModulePermissionDto dto)
    {
        return new RoleModulePermission
        {
            Id = dto.Id,
            RoleId = dto.RoleId,
            ModuleId = dto.ModuleId,
            CanView = dto.CanView,
            CanCreate = dto.CanCreate,
            CanEdit = dto.CanEdit,
            CanDelete = dto.CanDelete,
            // Note: CanApprove is not in the entity, ignore it
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    /// <summary>
    /// Updates an existing RoleModulePermission entity from RoleModulePermissionDto
    /// </summary>
    public static void UpdateEntity(this RoleModulePermissionDto dto, RoleModulePermission entity)
    {
        entity.RoleId = dto.RoleId;
        entity.ModuleId = dto.ModuleId;
        entity.CanView = dto.CanView;
        entity.CanCreate = dto.CanCreate;
        entity.CanEdit = dto.CanEdit;
        entity.CanDelete = dto.CanDelete;
        // Note: CanApprove is not in the entity, ignore it
        entity.UpdatedAt = dto.UpdatedAt;
    }

    /// <summary>
    /// Maps DiagnosticTestDto to DiagnosticTest entity
    /// </summary>
    public static DiagnosticTest ToEntity(this DiagnosticTestDto dto)
    {
        return new DiagnosticTest
        {
            Id = dto.Id,
            TestName = dto.TestName,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Duration = dto.Duration,
            RequiresFasting = dto.RequiresFasting,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    /// <summary>
    /// Updates an existing DiagnosticTest entity from DiagnosticTestDto
    /// </summary>
    public static void UpdateEntity(this DiagnosticTestDto dto, DiagnosticTest entity)
    {
        entity.TestName = dto.TestName;
        entity.Description = dto.Description;
        entity.Category = dto.Category;
        entity.Price = dto.Price;
        entity.Duration = dto.Duration;
        entity.RequiresFasting = dto.RequiresFasting;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = dto.UpdatedAt;
    }
}