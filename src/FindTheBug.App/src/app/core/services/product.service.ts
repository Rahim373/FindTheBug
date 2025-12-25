import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
    CreateProductRequest,
    UpdateProductRequest,
    ProductResponse,
    PagedProductsResponse
} from '../models/product.models';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getProducts(search?: string, pageNumber?: number, pageSize?: number): Observable<PagedProductsResponse> {
        let params = new HttpParams();
        if (search) {
            params = params.append('search', search);
        }
        if (pageNumber) {
            params = params.append('pageNumber', pageNumber.toString());
        }
        if (pageSize) {
            params = params.append('pageSize', pageSize.toString());
        }
        return this.http.get<PagedProductsResponse>(`${this.apiUrl}/products`, { params });
    }

    getProduct(id: string): Observable<ProductResponse> {
        return this.http.get<ProductResponse>(`${this.apiUrl}/products/${id}`);
    }

    createProduct(request: CreateProductRequest): Observable<ProductResponse> {
        return this.http.post<ProductResponse>(`${this.apiUrl}/products`, request);
    }

    updateProduct(id: string, request: UpdateProductRequest): Observable<ProductResponse> {
        return this.http.put<ProductResponse>(`${this.apiUrl}/products/${id}`, request);
    }

    deleteProduct(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/products/${id}`);
    }
}
