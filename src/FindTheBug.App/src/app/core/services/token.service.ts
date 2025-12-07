import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { jwtDecode  } from 'jwt-decode';


const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const TOKEN_EXPIRY_KEY = 'token_expiry';
const REFRESH_TOKEN_EXPIRY_KEY = 'refresh_token_expiry';

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

        const exp = jwtDecode(accessToken).exp;
        const expiration = exp ? exp * 1000 :  new Date();
        localStorage.setItem(TOKEN_EXPIRY_KEY, expiration.toString());
        localStorage.setItem(REFRESH_TOKEN_EXPIRY_KEY, new Date(expiresAt).toString());
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

        // Check if token is expired (with configured buffer time)
        const expiryDate = new Date(parseInt(expiryTime));
        const now = new Date();
        const bufferTime = environment.jwt.tokenBufferMinutes * 60 * 1000; // Convert minutes to milliseconds
        return now.getTime() < (expiryDate.getTime() - bufferTime);
    }

    /**
     * Clear all tokens from localStorage
     */
    clearTokens(): void {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
        localStorage.removeItem(TOKEN_EXPIRY_KEY);
        localStorage.removeItem(REFRESH_TOKEN_EXPIRY_KEY);
    }

    /**
     * Check if user is authenticated
     */
    isAuthenticated(): boolean {
        return this.isTokenValid();
    }
}
