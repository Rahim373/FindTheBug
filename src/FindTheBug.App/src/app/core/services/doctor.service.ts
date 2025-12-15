import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { 
  CreateDoctorRequest, 
  UpdateDoctorRequest,
  DoctorResponse,
  DoctorSpecialitiesResponse,
  PagedDoctorsResponse
} from '../models/doctor.models';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDoctors(search?: string): Observable<PagedDoctorsResponse> {
    let params = new HttpParams();
    if (search) {
      params = params.append('search', search);
    }
    return this.http.get<PagedDoctorsResponse>(`${this.apiUrl}/doctors`, { params });
  }

  getDoctor(id: string): Observable<DoctorResponse> {
    return this.http.get<DoctorResponse>(`${this.apiUrl}/doctors/${id}`);
  }

  createDoctor(request: CreateDoctorRequest): Observable<DoctorResponse> {
    return this.http.post<DoctorResponse>(`${this.apiUrl}/doctors`, request);
  }

  updateDoctor(id: string, request: UpdateDoctorRequest): Observable<DoctorResponse> {
    return this.http.put<DoctorResponse>(`${this.apiUrl}/doctors/${id}`, request);
  }

  deleteDoctor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/doctors/${id}`);
  }

  getDoctorSpecialities(): Observable<DoctorSpecialitiesResponse> {
    return this.http.get<DoctorSpecialitiesResponse>(`${this.apiUrl}/doctorspecialities`);
  }
}
