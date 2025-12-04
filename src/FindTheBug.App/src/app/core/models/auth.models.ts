// Authentication Models

export interface LoginRequest {
    emailOrPhone: string;
    password: string;
}

export interface LoginResponse extends Response<TokenData> {}

export interface TokenData {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    user: User | null;
}

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  roles: string
}

export interface ForgotPasswordRequest {
    email: string;
}

export interface ForgotPasswordResponse {
    isSuccess: boolean;
    data: any;
    errorMessage: string | null;
    errors: ErrorDetail[];
}

export interface ErrorDetail {
    code: string;
    description: string;
}

export interface User {
    id: string;
    email: string;
    name?: string;
}

export interface Response<T> {
    isSuccess: boolean;
    data: T | null;
    errorMessage: string | null,
    errors: any[];
}