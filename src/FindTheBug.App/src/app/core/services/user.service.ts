import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { User, CreateUserRequest, UpdateUserRequest, PagedResult } from '../models/user.models';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/users`;

    /**
     * Get paginated list of users with optional search
     */
    getUsers(pageNumber: number = 1, pageSize: number = 10, search?: string): Observable<any> {
        let params = new HttpParams()
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }

        return this.http.get<any>(this.apiUrl, { params });
    }

    /**
     * Get user by ID
     */
    getUserById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/${id}`);
    }

    /**
     * Create a new user
     */
    createUser(request: CreateUserRequest): Observable<any> {
        return this.http.post<any>(this.apiUrl, request);
    }

    /**
     * Update an existing user
     */
    updateUser(id: string, request: UpdateUserRequest): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${id}`, request);
    }

    /**
     * Delete a user
     */
    deleteUser(id: string): Observable<any> {
        return this.http.delete<any>(`${this.apiUrl}/${id}`);
    }
}
