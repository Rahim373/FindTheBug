import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { DiagnosticTestListItem } from '../models/diagnostic-test.models';
import { ApiResponse, PagedResult } from '../models/common.models';

@Injectable({
  providedIn: 'root'
})
export class DiagnosticTestService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDiagnosticTests(search?: string, category?: string, isActive?: boolean, pageNumber?: number, pageSize?: number): Observable<ApiResponse<PagedResult<DiagnosticTestListItem>>> {
    let params = new HttpParams();
    if (search) {
      params = params.append('search', search);
    }
    if (category) {
      params = params.append('category', category);
    }
    if (isActive !== undefined) {
      params = params.append('isActive', isActive.toString());
    }
    if (pageNumber) {
      params = params.append('pageNumber', pageNumber.toString());
    }
    if (pageSize) {
      params = params.append('pageSize', pageSize.toString());
    }
    return this.http.get<ApiResponse<PagedResult<DiagnosticTestListItem>>>(`${this.apiUrl}/diagnostictests`, { params });
  }
}