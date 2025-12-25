import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
    CreateDrugRequest,
    UpdateDrugRequest,
    DrugResponse,
    PagedDrugsResponse,
    BrandsResponse,
    GenericNamesResponse
} from '../models/drug.models';

@Injectable({
    providedIn: 'root'
})
export class DrugService {
    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getDrugs(search?: string, pageNumber?: number, pageSize?: number): Observable<PagedDrugsResponse> {
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
        return this.http.get<PagedDrugsResponse>(`${this.apiUrl}/drugs`, { params });
    }

    getDrug(id: string): Observable<DrugResponse> {
        return this.http.get<DrugResponse>(`${this.apiUrl}/drugs/${id}`);
    }

    createDrug(request: CreateDrugRequest): Observable<DrugResponse> {
        return this.http.post<DrugResponse>(`${this.apiUrl}/drugs`, request);
    }

    updateDrug(id: string, request: UpdateDrugRequest): Observable<DrugResponse> {
        return this.http.put<DrugResponse>(`${this.apiUrl}/drugs/${id}`, request);
    }

    deleteDrug(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/drugs/${id}`);
    }

    getBrands(): Observable<BrandsResponse> {
        return this.http.get<BrandsResponse>(`${this.apiUrl}/drugs/brands`);
    }

    getGenericNames(search?: string): Observable<GenericNamesResponse> {
        let params = new HttpParams();
        if (search) {
            params = params.append('search', search);
        }
        return this.http.get<GenericNamesResponse>(`${this.apiUrl}/drugs/generic-names`, { params });
    }
}
