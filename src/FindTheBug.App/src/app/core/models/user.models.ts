export interface UserRole {
  roleId: string;
  roleName: string;
}

export interface ModulePermission {
  module: string;
  permission: string;
}

export interface User {
  id: string;
  email?: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phone: string;
  nidNumber?: string;
  roleCount: number;
  isActive: boolean;
  allowUserLogin: boolean;
  lastLoginAt?: Date;
  createdAt: Date;
  updatedAt?: Date;
  roles?: UserRole[];
  permissions?: ModulePermission[];
}

export interface CreateUserRequest {
  email?: string;
  password: string;
  firstName: string;
  lastName: string;
  phone: string;
  nidNumber?: string;
  roleIds: string[];
  isActive: boolean;
  allowUserLogin: boolean;
}

export interface UpdateUserRequest {
  email?: string;
  firstName: string;
  lastName: string;
  phone: string;
  nidNumber?: string;
  roleIds: string[];
  isActive: boolean;
  allowUserLogin: boolean;
  password?: string;
}