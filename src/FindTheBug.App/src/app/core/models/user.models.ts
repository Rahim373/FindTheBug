export interface User {
  id: string;
  email?: string;
  firstName: string;
  lastName: string;
  phone?: string;
  nidNumber?: string;
  roles: string;
  isActive: boolean;
  lastLoginAt?: Date;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateUserRequest {
  email?: string;
  password: string;
  firstName: string;
  lastName: string;
  phone?: string;
  nidNumber?: string;
  roles?: string;
  isActive: boolean;
}

export interface UpdateUserRequest {
  email?: string;
  firstName: string;
  lastName: string;
  phone?: string;
  nidNumber?: string;
  roles?: string;
  isActive: boolean;
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
