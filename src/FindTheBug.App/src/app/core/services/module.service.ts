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

@Injectable({
    providedIn: 'root'
})
export class ModuleService {
    private readonly apiUrl = `${environment.apiUrl}/modules`;

    constructor(private http: HttpClient) { }

    async getModulesAsync(): Promise<Module[]> {
        try {
            const response = await firstValueFrom(this.http.get<any>(this.apiUrl));

            // Handle ResultWrapper format: {isSuccess: true, data: [...]}
            if (response?.data) {
                return Array.isArray(response.data) ? response.data : [];
            }

            // Fallback to direct array response
            return Array.isArray(response) ? response : [];
        } catch (error) {
            console.error('Error fetching modules:', error);
            throw error;
        }
    }
}
