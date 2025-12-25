import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { RoleService } from '../../../../core/services/role.service';
import { ModuleService, Module } from '../../../../core/services/module.service';
import { Role, CreateRoleRequest, UpdateRoleRequest, ModulePermission } from '../../../../core/models/role.models';

@Component({
    selector: 'app-role-form',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        NzFormModule,
        NzInputModule,
        NzButtonModule,
        NzSwitchModule,
        NzCheckboxModule,
        NzTableModule,
        NzCardModule,
        NzSpinModule
    ],
    templateUrl: './role-form.component.html',
    styleUrl: './role-form.component.css'
})
export class RoleFormComponent implements OnInit {
    roleForm!: FormGroup;
    isEditMode = false;
    roleId?: string;
    loading = false;
    submitting = false;
    modules: Module[] = [];
    // Track permissions per module: { moduleId: { view, create, edit, delete } }
    modulePermissions: Map<string, { view: boolean; create: boolean; edit: boolean; delete: boolean }> = new Map();

    constructor(
        private fb: FormBuilder,
        private roleService: RoleService,
        private moduleService: ModuleService,
        private router: Router,
        private route: ActivatedRoute,
        private message: NzMessageService
    ) { }

    ngOnInit(): void {
        this.initForm();
        this.loadData();
    }

    initForm(): void {
        this.roleForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(100)]],
            description: ['', [Validators.maxLength(500)]],
            isActive: [true]
        });
    }

    async loadData(): Promise<void> {
        this.loading = true;
        try {
            // Check if we're in edit mode first
            this.roleId = this.route.snapshot.paramMap.get('id') || undefined;
            this.isEditMode = !!this.roleId;

            // Load modules and role data in parallel for better performance
            const [modules, roleData] = await Promise.all([
                this.moduleService.getModulesAsync(),
                this.isEditMode && this.roleId ? this.roleService.getRoleByIdAsync(this.roleId) : Promise.resolve(null)
            ]);

            this.modules = modules;

            if (roleData) {
                this.roleForm.patchValue({
                    name: roleData.name,
                    description: roleData.description,
                    isActive: roleData.isActive
                });
                
                // Load existing module permissions
                if (roleData.modulePermissions && roleData.modulePermissions.length > 0) {
                    roleData.modulePermissions.forEach(permission => {
                        const moduleIdString = String(permission.moduleId);
                        this.modulePermissions.set(moduleIdString, {
                            view: permission.canView || false,
                            create: permission.canCreate || false,
                            edit: permission.canEdit || false,
                            delete: permission.canDelete || false
                        });
                    });
                }
            }
        } catch (error) {
            console.error('Error loading data:', error);
            this.message.error('Failed to load data. Please try again.');
        } finally {
            this.loading = false;
        }
    }

    isPermissionEnabled(moduleId: string, permission: 'view' | 'create' | 'edit' | 'delete'): boolean {
        return this.modulePermissions.get(moduleId)?.[permission] || false;
    }

    togglePermission(moduleId: string, permission: 'view' | 'create' | 'edit' | 'delete', value?: boolean): void {
        const currentPermissions = this.modulePermissions.get(moduleId) || { view: false, create: false, edit: false, delete: false };
        const newPermissions = { 
            ...currentPermissions, 
            [permission]: value !== undefined ? value : !currentPermissions[permission] 
        };
        this.modulePermissions.set(moduleId, newPermissions);
    }

    toggleAllPermissions(moduleId: string, enabled: boolean): void {
        this.modulePermissions.set(moduleId, { view: enabled, create: enabled, edit: enabled, delete: enabled });
    }

    hasAnyPermission(moduleId: string): boolean {
        const permissions = this.modulePermissions.get(moduleId);
        return permissions ? Object.values(permissions).some(p => p) : false;
    }

    toggleAllPermissionsForModule(moduleId: string): void {
        const permissions = this.modulePermissions.get(moduleId);
        if (permissions && Object.values(permissions).every(p => p)) {
            // All enabled, disable all
            this.toggleAllPermissions(moduleId, false);
        } else {
            // Not all enabled, enable all
            this.toggleAllPermissions(moduleId, true);
        }
    }

    isAllPermissionsEnabled(moduleId: string): boolean {
        const permissions = this.modulePermissions.get(moduleId);
        return permissions ? Object.values(permissions).every(p => p) : false;
    }

    async submitForm(): Promise<void> {
        if (this.roleForm.invalid) {
            Object.values(this.roleForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
            return;
        }

        this.submitting = true;
        try {
            const formValue = this.roleForm.value;

            // Convert module permissions to the request format
            const modulePermissions: ModulePermission[] = Array.from(this.modulePermissions.entries())
                .filter(([_, permissions]) => Object.values(permissions).some(p => p))
                .map(([moduleId, permissions]) => ({
                    moduleId,
                    canView: permissions.view,
                    canCreate: permissions.create,
                    canEdit: permissions.edit,
                    canDelete: permissions.delete
                }));

            const request: CreateRoleRequest | UpdateRoleRequest = {
                name: formValue.name,
                description: formValue.description,
                isActive: formValue.isActive,
                modulePermissions
            };

            if (this.isEditMode && this.roleId) {
                await this.roleService.updateRoleAsync(this.roleId, request as UpdateRoleRequest);
                this.message.success('Role updated successfully');
            } else {
                await this.roleService.createRoleAsync(request as CreateRoleRequest);
                this.message.success('Role created successfully');
            }

            this.router.navigate(['/admin/roles']);
        } catch (error) {
            console.error('Error saving role:', error);
            this.message.error(`Failed to ${this.isEditMode ? 'update' : 'create'} role. Please try again.`);
        } finally {
            this.submitting = false;
        }
    }

    cancel(): void {
        this.router.navigate(['/admin/roles']);
    }
}