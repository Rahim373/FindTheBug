import { Response } from './auth.models';
import { PagedResult } from './common.models';

export interface Product {
    id: string;
    name: string;
    price: number;
    quantity: number;
    description?: string;
    photoPath?: string;
    isActive: boolean;
    createdAt: string;
    updatedAt?: string;
}

export interface ProductListItem {
    id: string;
    name: string;
    price: number;
    quantity: number;
    description?: string;
    isActive: boolean;
}

export interface CreateProductRequest {
    name: string;
    price: number;
    quantity: number;
    description?: string;
    photoPath?: string;
}

export interface UpdateProductRequest {
    name: string;
    price: number;
    quantity: number;
    description?: string;
    photoPath?: string;
    isActive: boolean;
}

// Response wrappers
export interface ProductResponse extends Response<Product> { }
export interface PagedProductsResponse extends Response<PagedResult<ProductListItem>> { }
