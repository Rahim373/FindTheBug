import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzMessageService } from 'ng-zorro-antd/message';
import { RoleService } from '../../../../core/services/role.service';
import { ModuleService, Module } from '../../../../core/services/module.service';
import { ModulePermission } from '../../../../core/models/role.models';

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
    NzTableModule,
    NzCardModule,
    NzPageHeaderModule
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
  modulePermissions: { [key: string]: ModulePermission } = {};

  constructor(
    private fb: FormBuilder,
    private roleService: RoleService,
    private moduleService: ModuleService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadModules();
    this.roleId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.roleId;

    if (this.isEditMode && this.roleId) {
      this.loadRole(this.roleId);
    }
  }

  loadModules(): void {
    this.moduleService.getAll().subscribe({
      next: (modules) => {
        this.modules = modules;
        // Initialize module permissions for all modules
        modules.forEach(module => {
          this.modulePermissions[module.id] = {
            moduleId: module.id,
            canView: false,
            canCreate: false,
            canEdit: false,
            canDelete: false
          };
        });
      },
      error: () => {
        this.message.error('Failed to load modules');
      }
    });
  }

  getModulePermission(moduleId: string): ModulePermission | undefined {
    return this.modulePermissions[moduleId];
  }

  updateModulePermission(moduleId: string, permission: keyof ModulePermission, value: boolean): void {
    if (this.modulePermissions[moduleId]) {
      this.modulePermissions[moduleId] = {
        ...this.modulePermissions[moduleId],
        [permission]: value
      };
    }
  }

  initForm(): void {
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      isActive: [true]
    });
  }

  loadRole(id: string): void {
    this.loading = true;
    this.roleService.getById(id).subscribe({
      next: (role) => {
        this.roleForm.patchValue({
          name: role.name,
          description: role.description,
          isActive: role.isActive
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Failed to load role');
        this.router.navigate(['/admin/roles']);
      }
    });
  }

  submitForm(): void {
    if (this.roleForm.valid) {
      this.submitting = true;
      const formValue = this.roleForm.value;

      const request = {
        name: formValue.name,
        description: formValue.description,
        isActive: formValue.isActive,
        modulePermissions: Object.values(this.modulePermissions).filter(mp => 
          mp.canView || mp.canCreate || mp.canEdit || mp.canDelete
        )
      };

      const operation = this.isEditMode && this.roleId
        ? this.roleService.update(this.roleId, request)
        : this.roleService.create(request);

      operation.subscribe({
        next: () => {
          this.message.success(`Role ${this.isEditMode ? 'updated' : 'created'} successfully`);
          this.router.navigate(['/admin/roles']);
        },
        error: () => {
          this.submitting = false;
          this.message.error(`Failed to ${this.isEditMode ? 'update' : 'create'} role`);
        }
      });
    } else {
      Object.values(this.roleForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/admin/roles']);
  }
}
