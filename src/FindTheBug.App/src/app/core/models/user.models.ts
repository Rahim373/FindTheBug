export interface User {
  id: string;
  email?: string;
  firstName: string;
  lastName: string;
  phone: string;
  nidNumber?: string;
  roleIds: string[];
  isActive: boolean;
  allowUserLogin: boolean;
  lastLoginAt?: Date;
  createdAt: Date;
  updatedAt?: Date;
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

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
