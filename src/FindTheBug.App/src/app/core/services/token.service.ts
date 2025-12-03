import { Injectable } from '@angular/core';

const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const TOKEN_EXPIRY_KEY = 'token_expiry';

@Injectable({
    providedIn: 'root'
})
export class TokenService {

    /**
     * Store authentication tokens in localStorage
     */
    setTokens(accessToken: string, refreshToken: string, expiresAt: string): void {
        localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
        localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);

        // Calculate expiry timestamp
        const expiryTime = new Date(expiresAt);
        localStorage.setItem(TOKEN_EXPIRY_KEY, expiryTime.toString());
    }

    /**
     * Get access token from localStorage
     */
    getAccessToken(): string | null {
        return localStorage.getItem(ACCESS_TOKEN_KEY);
    }

    /**
     * Get refresh token from localStorage
     */
    getRefreshToken(): string | null {
        return localStorage.getItem(REFRESH_TOKEN_KEY);
    }

    /**
     * Check if access token exists and is not expired
     */
    isTokenValid(): boolean {
        const token = this.getAccessToken();
        if (!token) {
            return false;
        }

        const expiryTime = localStorage.getItem(TOKEN_EXPIRY_KEY);
        if (!expiryTime) {
            return false;
        }

        // Check if token is expired (with 1 minute buffer)
        return Date.now() < (parseInt(expiryTime) - 60000);
    }

    /**
     * Clear all tokens from localStorage
     */
    clearTokens(): void {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
        localStorage.removeItem(TOKEN_EXPIRY_KEY);
    }

    /**
     * Check if user is authenticated
     */
    isAuthenticated(): boolean {
        return this.isTokenValid();
    }
}
