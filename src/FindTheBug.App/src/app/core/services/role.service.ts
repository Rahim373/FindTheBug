import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { Role, CreateRoleRequest, UpdateRoleRequest } from '../models/role.models';
import { PagedResult } from '../models/common.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/roles`;

  constructor(private http: HttpClient) { }

  // Observable methods (for backward compatibility)
  getAll(search?: string, pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<Role>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<Role>>(this.apiUrl, { params });
  }

  getActive(): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.apiUrl}/active`);
  }

  getById(id: string): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateRoleRequest): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, request);
  }

  update(id: string, request: UpdateRoleRequest): Observable<Role> {
    return this.http.put<Role>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Async/await methods
  async getRolesAsync(search?: string, pageNumber: number = 1, pageSize: number = 10): Promise<PagedResult<Role>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return firstValueFrom(this.http.get<PagedResult<Role>>(this.apiUrl, { params }));
  }

  async getRoleByIdAsync(id: string): Promise<Role> {
    return firstValueFrom(this.http.get<Role>(`${this.apiUrl}/${id}`));
  }

  async createRoleAsync(request: CreateRoleRequest): Promise<Role> {
    return firstValueFrom(this.http.post<Role>(this.apiUrl, request));
  }

  async updateRoleAsync(id: string, request: UpdateRoleRequest): Promise<Role> {
    return firstValueFrom(this.http.put<Role>(`${this.apiUrl}/${id}`, request));
  }

  async deleteRoleAsync(id: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.apiUrl}/${id}`));
  }
}
