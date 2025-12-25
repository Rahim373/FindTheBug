import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export enum PermissionType {
    View = 'View',
    Create = 'Create',
    Edit = 'Edit',
    Delete = 'Delete'
}

export interface UserPermissions {
    [moduleName: string]: {
        view: boolean;
        create: boolean;
        edit: boolean;
        delete: boolean;
    };
}

@Injectable({
    providedIn: 'root'
})
export class PermissionService {
    private permissionsSubject = new BehaviorSubject<UserPermissions>({});
    permissions$ = this.permissionsSubject.asObservable();
    private cachedPermissions: UserPermissions = {};

    constructor(private authService: AuthService) {
        // Load permissions when user is authenticated
        this.authService.currentUser$.subscribe(user => {
            if (user && user.permissions) {
                this.updatePermissions(user.permissions);
            } else {
                this.clearPermissions();
            }
        });
    }

    /**
     * Check if user has a specific permission for a module
     * @param moduleName - Module name (e.g., 'Doctors', 'Patients', 'Laboratory', etc.)
     * @param permission - Permission type to check
     * @returns Observable<boolean>
     */
    hasPermission(moduleName: string, permission: PermissionType): Observable<boolean> {
        const permissionKey = this.getModuleKey(moduleName);
        const hasPermission = this.cachedPermissions[permissionKey]?.[permission.toLowerCase() as keyof UserPermissions[string]] || false;
        return of(hasPermission);
    }

    /**
     * Synchronous check for permission (use in templates)
     * @param moduleName - Module name (e.g., 'Doctors', 'Patients', 'Laboratory', etc.)
     * @param permission - Permission type to check
     * @returns boolean
     */
    hasPermissionSync(moduleName: string, permission: PermissionType): boolean {
        const permissionKey = this.getModuleKey(moduleName);
        return this.cachedPermissions[permissionKey]?.[permission.toLowerCase() as keyof UserPermissions[string]] || false;
    }

    /**
     * Check if user has any permission for a module
     * @param moduleName - Module name
     * @returns Observable<boolean>
     */
    hasAnyPermission(moduleName: string): Observable<boolean> {
        const permissionKey = this.getModuleKey(moduleName);
        const modulePermissions = this.cachedPermissions[permissionKey];
        const hasAny = modulePermissions ? Object.values(modulePermissions).some(p => p) : false;
        return of(hasAny);
    }

    /**
     * Synchronous check for any permission (use in templates)
     * @param moduleName - Module name
     * @returns boolean
     */
    hasAnyPermissionSync(moduleName: string): boolean {
        const permissionKey = this.getModuleKey(moduleName);
        const modulePermissions = this.cachedPermissions[permissionKey];
        return modulePermissions ? Object.values(modulePermissions).some(p => p) : false;
    }

    /**
     * Update permissions from user data
     * @param permissions - User permissions from API response
     */
    private updatePermissions(permissions: any[]): void {
        const permissionsMap: UserPermissions = {};
        
        if (permissions && Array.isArray(permissions)) {
            permissions.forEach(perm => {
                // Handle new format: { module: "Doctors", permission: "View" }
                if (perm.module && perm.permission) {
                    const key = perm.module;
                    if (!permissionsMap[key]) {
                        permissionsMap[key] = {
                            view: false,
                            create: false,
                            edit: false,
                            delete: false
                        };
                    }
                    
                    // Map permission string to boolean property
                    const permission = perm.permission.toLowerCase();
                    if (permission === 'view') permissionsMap[key].view = true;
                    if (permission === 'create') permissionsMap[key].create = true;
                    if (permission === 'edit') permissionsMap[key].edit = true;
                    if (permission === 'delete') permissionsMap[key].delete = true;
                }
                // Handle old format for backward compatibility: { moduleName: "Doctors", canView: true, ... }
                else if (perm.moduleName || perm.moduleId || perm.module) {
                    const key = perm.moduleName || perm.moduleId || perm.module;
                    if (key) {
                        permissionsMap[key] = {
                            view: perm.canView || false,
                            create: perm.canCreate || false,
                            edit: perm.canEdit || false,
                            delete: perm.canDelete || false
                        };
                    }
                }
            });
        }
        
        this.cachedPermissions = permissionsMap;
        this.permissionsSubject.next(permissionsMap);
    }

    /**
     * Clear permissions (on logout)
     */
    private clearPermissions(): void {
        this.cachedPermissions = {};
        this.permissionsSubject.next({});
    }

    /**
     * Get module key (normalize module name)
     */
    private getModuleKey(moduleName: string): string {
        // Map module names to keys
        const moduleMap: { [key: string]: string } = {
            'Doctors': 'Doctors',
            'Patients': 'Patients',
            'Laboratory': 'Laboratory',
            'Dispensary': 'Dispensary',
            'Billing': 'Billing',
            'UserManagement': 'UserManagement',
            'Users': 'UserManagement',
            'Roles': 'UserManagement'
        };
        return moduleMap[moduleName] || moduleName;
    }
}