# Module-Based Authorization Guide

## Overview

FindTheBug implements a comprehensive module-based authorization system that allows granular control over user access to different features and functionalities. Users are assigned roles, and roles are granted specific permissions for each module.

## Architecture

### Backend Components

#### 1. Domain Layer (`FindTheBug.Domain`)

**ModulePermission Enum** (`Common/ModulePermission.cs`):
```csharp
public enum ModulePermission
{
    None = 0,
    View = 1,
    Create = 2,
    Edit = 3,
    Delete = 4
}
```

#### 2. Application Layer (`FindTheBug.Application`)

**IModulePermissionService** (`Common/Interfaces/IModulePermissionService.cs`):
- `HasPermissionAsync(Guid userId, string moduleName, ModulePermission permission)`: Check if user has specific permission
- `GetUserPermissionsAsync(Guid userId)`: Get all user permissions

#### 3. Infrastructure Layer (`FindTheBug.Infrastructure`)

**ModulePermissionService** (`Services/ModulePermissionService.cs`):
- Retrieves user roles and their module permissions
- Aggregates permissions across multiple roles
- Caches permission data for performance

#### 4. WebAPI Layer (`FindTheBug.WebAPI`)

**Authorization Attribute** (`Attributes/RequireModulePermissionAttribute.cs`):
```csharp
[RequireModulePermission("Doctors", ModulePermission.View)]
public async Task<IActionResult> GetAllDoctors() { ... }
```

**Authorization Handler** (`Authorization/ModulePermissionHandler.cs`):
- Evaluates permissions at runtime
- Returns 403 Forbidden if user lacks required permission

**Authorization Policies** (`Extensions/AuthorizationExtensions.cs`):
- Registers policies for all modules and permissions
- Format: `Module_{ModuleName}_{Permission}`

### Frontend Components

#### 1. Permission Service (`src/FindTheBug.App/src/app/core/services/permission.service.ts`)

**PermissionType Enum**:
```typescript
export enum PermissionType {
    View = 'View',
    Create = 'Create',
    Edit = 'Edit',
    Delete = 'Delete'
}
```

**PermissionService Methods**:
- `hasPermission(moduleName, permission: PermissionType)`: Observable check
- `hasPermissionSync(moduleName, permission: PermissionType)`: Synchronous check (for templates)
- `hasAnyPermission(moduleName)`: Check if user has any permission for module
- `hasAnyPermissionSync(moduleName)`: Synchronous check (for templates)

#### 2. Permission Directive (`src/FindTheBug.App/src/app/core/directives/has-permission.directive.ts`)

**Usage in Templates**:
```html
<ng-template *hasPermission="'Doctors'; let hasPermissionType: PermissionType = PermissionType.View">
    <button>Edit Doctor</button>
</ng-template>

<ng-template *hasPermission="'Doctors'; let hasPermissionType: PermissionType = PermissionType.Create">
    <button>Create Doctor</button>
</ng-template>

<ng-template *hasPermission="'Doctors'; let hasPermissionType: PermissionType = PermissionType.Edit; else: noEditPermission">
    <button>Edit Doctor</button>
</ng-template>
<ng-template #noEditPermission>
    <span>No permission to edit</span>
</ng-template>
```

## Module Structure

The system currently supports the following modules:

| Module | Permissions | Description |
|---------|-------------|-------------|
| **Doctors** | View, Create, Edit, Delete | Doctor management |
| **Patients** | View, Create, Edit, Delete | Patient management |
| **Laboratory** | View, Create, Edit, Delete | Diagnostic tests, test entries, test parameters, test results |
| **Dispensary** | View, Create, Edit, Delete | Drugs and products management |
| **Billing** | View, Create, Edit, Delete | Invoice management |
| **UserManagement** | View, Create, Edit, Delete | Users and roles management |

## How It Works

### Backend Flow

1. **Authentication**:
   - User logs in with credentials
   - JWT token is generated with user ID claim

2. **Authorization**:
   - Each API endpoint has `[RequireModulePermission("ModuleName", ModulePermission.Level)]` attribute
   - Attribute generates policy name: `Module_ModuleName_Level`
   - Example: `Module_Doctors_View`

3. **Permission Evaluation**:
   ```
   User Request ’ JWT Authentication ’ ModulePermissionHandler
                                    “
                             Get User Roles
                                    “
                             Get Role Permissions
                                    “
                             Aggregate Permissions
                                    “
                             Check Required Permission
                                    “
                             Allow/Deny Access (403)
   ```

4. **Permission Aggregation**:
   - User can have multiple roles
   - Permissions are aggregated from all roles
   - Union of all permissions from all roles

### Frontend Flow

1. **User Login**:
   - User authenticates
   - Auth service stores user data including permissions
   - Permission service loads and caches permissions

2. **Permission Checking**:
   - Components check permissions using `PermissionService`
   - Menu items hide/show based on permissions
   - UI controls (buttons, links) respect permissions

3. **UI Updates**:
   - Sidebar menus only show accessible modules
   - Action buttons hide if user lacks permission
   - Navigation is protected

## Usage Examples

### Backend

#### 1. Controller Setup

```csharp
[ApiController]
[Route("api/doctors")]
public class DoctorsController : BaseApiController
{
    [HttpGet]
    [RequireModulePermission("Doctors", ModulePermission.View)]
    public async Task<IActionResult> GetAll() { ... }

    [HttpGet("{id}")]
    [RequireModulePermission("Doctors", ModulePermission.View)]
    public async Task<IActionResult> GetById(Guid id) { ... }

    [HttpPost]
    [RequireModulePermission("Doctors", ModulePermission.Create)]
    public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command) { ... }

    [HttpPut("{id}")]
    [RequireModulePermission("Doctors", ModulePermission.Edit)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorCommand command) { ... }

    [HttpDelete("{id}")]
    [RequireModulePermission("Doctors", ModulePermission.Delete)]
    public async Task<IActionResult> Delete(Guid id) { ... }
}
```

#### 2. Custom Authorization Check

```csharp
public class MyService
{
    private readonly IModulePermissionService _permissionService;

    public async Task<bool> CanUserEditDoctorAsync(Guid userId, Guid doctorId)
    {
        return await _permissionService.HasPermissionAsync(userId, "Doctors", ModulePermission.Edit);
    }
}
```

### Frontend

#### 1. Sidebar Menu with Permissions

```typescript
// sidebar.component.ts
import { PermissionService, PermissionType } from '../../../../core/services/permission.service';

export class SidebarComponent {
    permissionService = inject(PermissionService);
    
    // In template
    <li nz-submenu *ngIf="permissionService.hasAnyPermissionSync('Doctors')">
        <ul>
            <li *ngIf="permissionService.hasPermissionSync('Doctors', PermissionType.View)">
                <a routerLink="/admin/doctors">Doctors</a>
            </li>
        </ul>
    </li>
}
```

#### 2. Component with Permission Checks

```typescript
// doctors.component.ts
export class DoctorsComponent {
    permissionService = inject(PermissionService);
    
    canView = false;
    canCreate = false;
    canEdit = false;
    canDelete = false;
    
    ngOnInit() {
        this.canView = this.permissionService.hasPermissionSync('Doctors', PermissionType.View);
        this.canCreate = this.permissionService.hasPermissionSync('Doctors', PermissionType.Create);
        this.canEdit = this.permissionService.hasPermissionSync('Doctors', PermissionType.Edit);
        this.canDelete = this.permissionService.hasPermissionSync('Doctors', PermissionType.Delete);
    }
}
```

#### 3. Template with Conditional UI

```html
<!-- Using permission service -->
<button *ngIf="permissionService.hasPermissionSync('Doctors', PermissionType.Edit)">
    Edit Doctor
</button>

<button *ngIf="permissionService.hasPermissionSync('Doctors', PermissionType.Delete)">
    Delete Doctor
</button>

<!-- Using permission directive -->
<ng-container *hasPermission="'Doctors'; let hasPermissionType: PermissionType = PermissionType.Create">
    <button>Create Doctor</button>
</ng-container>
```

#### 4. Role Form with Module Permissions

The Role Management form provides a table-based interface for configuring module permissions:

```
                ,     ,       ,      ,       ,        
 Module         All   View    Edit   Create  Delete 
                <     <       <      <       <        $
 Doctors                                   
 Patients                                  
 Laboratory                                
 Dispensary                                
                4     4       4      4       4        
```

- **All Column**: Quick toggle for all permissions
- **Individual Columns**: Fine-grained control per permission type

## Testing

### Backend Testing

1. **Test Permission Granted**:
```bash
# Create role with Doctor View permission
# Assign role to user
# Call GET /api/doctors
# Expected: 200 OK
```

2. **Test Permission Denied**:
```bash
# User doesn't have Doctor Delete permission
# Call DELETE /api/doctors/{id}
# Expected: 403 Forbidden
```

3. **Test Permission Aggregation**:
```bash
# User has Role1 (Doctor: View) and Role2 (Doctor: Create)
# Call POST /api/doctors
# Expected: 200 OK (has Create permission from Role2)
```

### Frontend Testing

1. **Test Menu Visibility**:
   - Login as user with specific permissions
   - Verify only accessible modules show in sidebar

2. **Test Button Visibility**:
   - Verify edit/delete buttons hide for users without permissions

3. **Test Role Management**:
   - Create role with specific module permissions
   - Verify permissions are correctly saved and applied

## Best Practices

### Backend

1. **Always use attribute**: Apply `[RequireModulePermission]` to all protected endpoints
2. **Use appropriate permissions**: 
   - `View`: GET requests
   - `Create`: POST requests
   - `Edit`: PUT/PATCH requests
   - `Delete`: DELETE requests
3. **Aggregation is automatic**: Don't manually aggregate permissions - use the service

### Frontend

1. **Check permissions early**: Check permissions in `ngOnInit` or constructor
2. **Use sync checks in templates**: Use `hasPermissionSync()` for better performance
3. **Hide vs disable**: Hide elements completely if no permission (don't just disable)
4. **Provide feedback**: Show access denied message when permission check fails

### Security

1. **Defense in depth**: 
   - Frontend checks for UX
   - Backend enforces security
2. **Never trust frontend**: Always validate on backend
3. **Audit logging**: Log permission denials for security monitoring

## Troubleshooting

### Common Issues

1. **User can't access module**:
   - Check user has correct role assigned
   - Verify role has module permissions
   - Check permissions are not conflicting

2. **Frontend not showing menu**:
   - Verify permissions are loaded in auth service
   - Check permission service is caching correctly
   - Use browser dev tools to check user object

3. **403 Forbidden errors**:
   - Check JWT token includes user ID claim
   - Verify authorization handler is registered
   - Check policy name format matches attribute

## Migration Guide

If migrating from role-based to module-based authorization:

1. **Map existing roles to modules**:
   ```csharp
   // Example migration
   // Old: Admin role
   // New: Admin role with all modules, all permissions
   
   // Old: Doctor role
   // New: Doctor role with Doctors module (View, Edit, Create)
   ```

2. **Update all controllers**:
   - Add `[RequireModulePermission]` attributes
   - Remove role-based checks

3. **Update frontend**:
   - Inject `PermissionService`
   - Update sidebar menu
   - Add permission checks to actions

## Future Enhancements

Potential improvements to the authorization system:

1. **Field-level permissions**: Grant access to specific fields (e.g., view but not edit sensitive data)
2. **Row-level permissions**: Grant access only to records created by user or their department
3. **Time-based permissions**: Temporary access grants (e.g., audit access for 24 hours)
4. **Dynamic permissions**: Permission calculation based on business rules
5. **Permission templates**: Pre-configured permission sets for common roles

## Support

For issues or questions about module-based authorization:
1. Check this documentation
2. Review `ModulePermissionService` implementation
3. Check authorization logs in the application
4. Verify user roles and permissions in database