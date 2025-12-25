import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { UserService } from '../../../../core/services/user.service';
import { RoleService } from '../../../../core/services/role.service';
import { CreateUserRequest, UpdateUserRequest } from '../../../../core/models/user.models';
import { Role } from '../../../../core/models/role.models';

@Component({
    selector: 'app-user-form',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        NzFormModule,
        NzInputModule,
        NzButtonModule,
        NzSwitchModule,
        NzSelectModule,
        NzCardModule
    ],
    templateUrl: './user-form.component.html',
    styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly userService = inject(UserService);
    private readonly roleService = inject(RoleService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly message = inject(NzMessageService);

    userForm!: FormGroup;
    isEditMode = false;
    userId?: string;
    loading = false;
    submitting = false;
    availableRoles: Role[] = [];
    loadingRoles = false;

    ngOnInit(): void {
        this.userId = this.route.snapshot.paramMap.get('id') || undefined;
        this.isEditMode = !!this.userId;

        this.initForm();
        this.loadRoles();

        if (this.isEditMode && this.userId) {
            this.loadUser(this.userId);
        }
    }

    initForm(): void {
        this.userForm = this.fb.group({
            email: ['', [Validators.email]],
            password: [''],
            firstName: ['', [Validators.required, Validators.maxLength(100)]],
            lastName: ['', [Validators.required, Validators.maxLength(100)]],
            phone: ['', [Validators.required]],
            nidNumber: ['', [Validators.maxLength(50)]],
            roleIds: [[], [Validators.required]],
            isActive: [true],
            allowUserLogin: [true]
        });

        // Add dynamic password validation based on allowUserLogin
        this.userForm.get('allowUserLogin')?.valueChanges.subscribe(allowLogin => {
            const passwordControl = this.userForm.get('password');
            if (allowLogin) {
                // Password required when allowUserLogin is true (only for create mode)
                if (!this.isEditMode) {
                    passwordControl?.setValidators([Validators.required, Validators.minLength(6)]);
                } else {
                    passwordControl?.setValidators([Validators.minLength(6)]);
                }
            } else {
                // Password not required when allowUserLogin is false
                passwordControl?.clearValidators();
            }
            passwordControl?.updateValueAndValidity();
        });

        // Set initial validators based on mode
        const passwordControl = this.userForm.get('password');
        if (!this.isEditMode) {
            passwordControl?.setValidators([Validators.required, Validators.minLength(6)]);
        } else {
            passwordControl?.setValidators([Validators.minLength(6)]);
        }
    }

    loadRoles(): void {
        this.loadingRoles = true;
        this.roleService.getActive().subscribe({
            next: (roles) => {
                this.availableRoles = roles;
                this.loadingRoles = false;
            },
            error: () => {
                this.message.error('Failed to load roles');
                this.loadingRoles = false;
            }
        });
    }

    loadUser(id: string): void {
        this.loading = true;
        this.userService.getUserById(id).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    const user = response.data;
                    // Map roles array to roleIds array
                    //const roleIds = user.roles?.map(r => r.roleId) || [];
                    this.userForm.patchValue({
                        email: user.email,
                        firstName: user.firstName,
                        lastName: user.lastName,
                        phone: user.phone,
                        nidNumber: user.nidNumber,
                        //roleIds: roleIds,
                        isActive: user.isActive,
                        allowUserLogin: user.allowUserLogin
                    });
                }
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading user:', error);
                this.message.error('Failed to load user');
                this.loading = false;
            }
        });
    }

    onSubmit(): void {
        if (this.userForm.invalid) {
            Object.values(this.userForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
            return;
        }

        this.submitting = true;
        const formValue = this.userForm.value;

        if (this.isEditMode && this.userId) {
            const request: UpdateUserRequest = {
                email: formValue.email,
                firstName: formValue.firstName,
                lastName: formValue.lastName,
                phone: formValue.phone,
                nidNumber: formValue.nidNumber,
                roleIds: formValue.roleIds,
                isActive: formValue.isActive,
                allowUserLogin: formValue.allowUserLogin,
                password: formValue.password || undefined
            };

            this.userService.updateUser(this.userId, request).subscribe({
                next: (response) => {
                    if (response.isSuccess) {
                        this.message.success('User updated successfully');
                        this.router.navigate(['/admin/users']);
                    } else {
                        this.message.error(response.errors?.[0]?.description || 'Failed to update user');
                    }
                    this.submitting = false;
                },
                error: (error) => {
                    console.error('Error updating user:', error);
                    this.message.error('Failed to update user');
                    this.submitting = false;
                }
            });
        } else {
            const request: CreateUserRequest = {
                email: formValue.email,
                password: formValue.password,
                firstName: formValue.firstName,
                lastName: formValue.lastName,
                phone: formValue.phone,
                nidNumber: formValue.nidNumber,
                roleIds: formValue.roleIds,
                isActive: formValue.isActive,
                allowUserLogin: formValue.allowUserLogin
            };

            this.userService.createUser(request).subscribe({
                next: (response) => {
                    if (response.isSuccess) {
                        this.message.success('User created successfully');
                        this.router.navigate(['/admin/users']);
                    } else {
                        this.message.error(response.errors?.[0]?.description || 'Failed to create user');
                    }
                    this.submitting = false;
                },
                error: (error) => {
                    console.error('Error creating user:', error);
                    this.message.error('Failed to create user');
                    this.submitting = false;
                }
            });
        }
    }

    cancel(): void {
        this.router.navigate(['/admin/users']);
    }

    getPasswordPlaceholder(): string {
        const allowLogin = this.userForm.get('allowUserLogin')?.value;
        if (this.isEditMode) {
            return allowLogin ? 'Leave blank to keep current password' : 'Password not required (login disabled)';
        }
        return allowLogin ? 'Password' : 'Password not required (login disabled)';
    }

    getPasswordErrorTip(): string {
        const allowLogin = this.userForm.get('allowUserLogin')?.value;
        if (!allowLogin) {
            return 'Password not required when login is disabled';
        }
        if (this.isEditMode) {
            return 'Password must be at least 6 characters';
        }
        return 'Please enter password (min 6 characters)';
    }
}