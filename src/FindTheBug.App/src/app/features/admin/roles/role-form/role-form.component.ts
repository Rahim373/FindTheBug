import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzMessageService } from 'ng-zorro-antd/message';
import { RoleService } from '../../../../core/services/role.service';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSwitchModule
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

  constructor(
    private fb: FormBuilder,
    private roleService: RoleService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.roleId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.roleId;

    if (this.isEditMode && this.roleId) {
      this.loadRole(this.roleId);
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
        isActive: formValue.isActive
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
