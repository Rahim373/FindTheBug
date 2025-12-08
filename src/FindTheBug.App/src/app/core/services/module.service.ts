import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment.development';

export interface Module {
    id: string;
    name: string;
    displayName?: string;
    description?: string;
    isActive: boolean;
}

interface ApiResponse<T> {
    data?: T;
    success?: boolean;
    message?: string;
}

@Injectable({
    providedIn: 'root'
})
export class ModuleService {
    private readonly apiUrl = `${environment.apiUrl}/modules`;

    constructor(private http: HttpClient) { }

    async getModulesAsync(): Promise<Module[]> {
        try {
            const response = await firstValueFrom(this.http.get<any>(this.apiUrl));
            return this.extractData(response);
        } catch (error) {
            console.error('Error fetching modules:', error);
            throw error;
        }
    }

    private extractData(response: any): Module[] {
        // Handle wrapped response {data: [...]}
        if (response && response.data && Array.isArray(response.data)) {
            return response.data;
        }
        // Handle direct array response [...]
        if (Array.isArray(response)) {
            return response;
        }
        // Fallback to empty array
        console.warn('Unexpected module response format:', response);
        return [];
    }
}
