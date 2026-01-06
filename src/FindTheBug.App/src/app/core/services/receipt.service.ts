import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
  CreateReceiptRequest,
  UpdateReceiptRequest,
  Receipt,
  PagedReceiptsResult
} from '../models/receipt.models';

@Injectable({
  providedIn: 'root'
})
export class ReceiptService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getReceipts(search?: string, pageNumber?: number, pageSize?: number): Observable<PagedReceiptsResult> {
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
    return this.http.get<PagedReceiptsResult>(`${this.apiUrl}/receipts`, { params });
  }

  getReceipt(id: string): Observable<Receipt> {
    return this.http.get<Receipt>(`${this.apiUrl}/receipts/${id}`);
  }

  createReceipt(request: CreateReceiptRequest): Observable<Receipt> {
    return this.http.post<Receipt>(`${this.apiUrl}/receipts`, request);
  }

  updateReceipt(id: string, request: UpdateReceiptRequest): Observable<Receipt> {
    return this.http.put<Receipt>(`${this.apiUrl}/receipts/${id}`, request);
  }

  deleteReceipt(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/receipts/${id}`);
  }
}