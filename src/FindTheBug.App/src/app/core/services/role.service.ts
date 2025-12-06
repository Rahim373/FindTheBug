import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Role, CreateRoleRequest, UpdateRoleRequest } from '../models/role.models';
import { PagedResult } from '../models/common.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/roles`;

  constructor(private http: HttpClient) {}

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
}
