import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { TokenService } from './token.service';
import { LoginRequest, LoginResponse, ForgotPasswordRequest, ForgotPasswordResponse } from '../models/auth.models';
import { environment } from '../../../environments/environment.development';
import { firstValueFrom } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly tokenService = inject(TokenService);
    private readonly router = inject(Router);

    private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.tokenService.isAuthenticated());
    public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

    /**
     * Login with email/phone and password
     */
    async login(emailOrPhone: string, password: string): Promise<LoginResponse> {
        const request: LoginRequest = { emailOrPhone, password };

        try {
            const response = await firstValueFrom(
                this.http.post<LoginResponse>(
                    `${environment.apiUrl}/token`,
                    request
                )
            );

            if (!response) {
                throw new Error('No response from server');
            }

            if (response.isSuccess && response.data) {
                // Store tokens
                this.tokenService.setTokens(
                    response.data.accessToken,
                    response.data.refreshToken,
                    response.data.expiresAt
                );

                // Update authentication state
                this.isAuthenticatedSubject.next(true);
            }

            return response;
        } catch (error: any) {
            console.error('Login error:', error);
            throw error;
        }
    }

    /**
     * Request password reset
     */
    async requestPasswordReset(email: string): Promise<ForgotPasswordResponse> {
        const request: ForgotPasswordRequest = { email };

        try {
            const response = await firstValueFrom(
                this.http.post<ForgotPasswordResponse>(
                    `${environment.apiUrl}/token/request-reset`,
                    request
                )
            );

            if (!response) {
                throw new Error('No response from server');
            }

            return response;
        } catch (error: any) {
            console.error('Password reset request error:', error);
            throw error;
        }
    }

    /**
     * Logout user
     */
    async logout(): Promise<void> {
        const refreshToken = this.tokenService.getRefreshToken();

        if (refreshToken) {
            try {
                // Call revoke endpoint
                await firstValueFrom(
                    this.http.post(
                        `${environment.apiUrl}/token/revoke`,
                        { refreshToken }
                    )
                );
            } catch (error) {
                console.error('Error revoking token:', error);
            }
        }

        // Clear tokens and update state
        this.tokenService.clearTokens();
        this.isAuthenticatedSubject.next(false);

        // Redirect to login
        await this.router.navigate(['/login']);
    }

    /**
     * Check if user is authenticated
     */
    isAuthenticated(): boolean {
        return this.tokenService.isAuthenticated();
    }

    /**
     * Check if user is on login page and redirect to dashboard if authenticated
     */
    async checkAuthAndRedirect(): Promise<void> {
        if (this.isAuthenticated()) {
            await this.router.navigate(['/admin/dashboard']);
        }
    }
}
