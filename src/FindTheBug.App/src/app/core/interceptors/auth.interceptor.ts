import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError, Subject, of, switchMap, filter, take, from } from 'rxjs';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';

// Track ongoing refresh requests and retry count
let isRefreshing = false;
let refreshTokenSubject: Subject<boolean> = new Subject<boolean>();
let retryCount = 0;
const MAX_RETRY_ATTEMPTS = 3;

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    const tokenService = inject(TokenService);
    const authService = inject(AuthService);
    const router = inject(Router);
    const token = tokenService.getAccessToken();

    let authReq = req;

    // Add Bearer token if available
    if (token) {
        authReq = req.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    }

    return next(authReq).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                // Token expired or invalid - try to refresh
                if (!isRefreshing) {
                    isRefreshing = true;
                    refreshTokenSubject.next(false);

                    return from(authService.refreshAccessToken()).pipe(
                        switchMap((response) => {
                            isRefreshing = false;
                            if (response) {
                                // Token refreshed successfully
                                refreshTokenSubject.next(true);
                                retryCount = 0; // Reset retry count on success
                                
                                // Retry the original request with new token
                                const newToken = tokenService.getAccessToken();
                                const retryReq = req.clone({
                                    setHeaders: {
                                        Authorization: `Bearer ${newToken}`
                                    }
                                });
                                return next(retryReq);
                            } else {
                                // Refresh failed - try again if we haven't exceeded max attempts
                                if (retryCount < MAX_RETRY_ATTEMPTS) {
                                    retryCount++;
                                    isRefreshing = false;
                                    return next(req); // Retry the original request
                                } else {
                                    // Max attempts reached - redirect to login
                                    tokenService.clearTokens();
                                    router.navigate(['/login']);
                                    return throwError(() => error);
                                }
                            }
                        }),
                        catchError((refreshError) => {
                            isRefreshing = false;
                            if (retryCount < MAX_RETRY_ATTEMPTS) {
                                retryCount++;
                                return next(req); // Retry the original request
                            } else {
                                // Max attempts reached - redirect to login
                                tokenService.clearTokens();
                                router.navigate(['/login']);
                                return throwError(() => refreshError);
                            }
                        })
                    );
                } else {
                    // Wait for the ongoing refresh to complete
                    return refreshTokenSubject.pipe(
                        filter(isRefreshed => isRefreshed),
                        take(1),
                        switchMap(() => {
                            // Retry the original request with new token
                            const newToken = tokenService.getAccessToken();
                            const retryReq = req.clone({
                                setHeaders: {
                                    Authorization: `Bearer ${newToken}`
                                }
                            });
                            return next(retryReq);
                        })
                    );
                }
            }
            return throwError(() => error);
        })
    );
};
