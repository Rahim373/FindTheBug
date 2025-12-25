namespace FindTheBug.Domain.Common;

/// <summary>
/// Module permissions that can be granted to roles
/// </summary>
[Flags]
public enum ModulePermission
{
    None = 0,
    View = 1,
    Create = 2,
    Edit = 4,
    Delete = 8,
    All = View | Create | Edit | Delete
}