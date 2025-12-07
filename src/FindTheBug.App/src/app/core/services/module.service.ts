import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
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

  constructor(private http: HttpClient) {}

  async getAll(): Promise<Module[]> {
    return firstValueFrom(this.http.get<Module[]>(this.apiUrl));
  }

  getAllObservable(): Observable<Module[]> {
    return this.http.get<Module[]>(this.apiUrl);
  }
}
