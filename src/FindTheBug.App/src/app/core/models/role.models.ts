export interface Role {
  id: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  isActive: boolean;
  createdAt: Date;
  modulePermissions?: ModulePermission[];
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
  isActive: boolean;
  modulePermissions?: ModulePermission[];
}

export interface UpdateRoleRequest {
  name: string;
  description?: string;
  isActive: boolean;
  modulePermissions?: ModulePermission[];
}

export interface ModulePermission {
  moduleId: string;
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}