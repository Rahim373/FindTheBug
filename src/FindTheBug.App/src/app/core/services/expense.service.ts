import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
    CreateExpenseRequest,
    UpdateExpenseRequest,
    Expense,
    PaginatedExpenseList
} from '../models/expense.models';
import { ApiResponse } from '../models/common.models';

@Injectable({
    providedIn: 'root'
})
export class ExpenseService {
    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getExpenses(
        department?: string,
        search?: string,
        pageNumber?: number,
        pageSize?: number
    ): Observable<ApiResponse<PaginatedExpenseList>> {
        let params = new HttpParams();
        if (department) {
            params = params.append('department', department);
        }
        if (search) {
            params = params.append('search', search);
        }
        if (pageNumber) {
            params = params.append('pageNumber', pageNumber.toString());
        }
        if (pageSize) {
            params = params.append('pageSize', pageSize.toString());
        }
        return this.http.get<ApiResponse<PaginatedExpenseList>>(`${this.apiUrl}/expenses`, { params });
    }

    getExpense(id: string): Observable<ApiResponse<Expense>> {
        return this.http.get<ApiResponse<Expense>>(`${this.apiUrl}/expenses/${id}`);
    }

    createExpense(request: CreateExpenseRequest): Observable<ApiResponse<Expense>> {
        return this.http.post<ApiResponse<Expense>>(`${this.apiUrl}/expenses`, request);
    }

    updateExpense(request: UpdateExpenseRequest): Observable<ApiResponse<Expense>> {
        return this.http.put<ApiResponse<Expense>>(`${this.apiUrl}/expenses`, request);
    }

    deleteExpense(id: string): Observable<ApiResponse<void>> {
        return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/expenses/${id}`);
    }
}