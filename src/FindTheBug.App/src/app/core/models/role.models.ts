export interface Role {
  id: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  isActive: boolean;
  createdAt: Date;
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
  isActive: boolean;
}

export interface UpdateRoleRequest {
  name: string;
  description?: string;
  isActive: boolean;
}
