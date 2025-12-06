import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FormsModule } from '@angular/forms';
import { RoleService } from '../../../../core/services/role.service';
import { Role } from '../../../../core/models/role.models';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzTagModule,
    NzModalModule
  ],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.css'
})
export class RolesListComponent implements OnInit {
  roles: Role[] = [];
  loading = false;
  searchValue = '';
  pageIndex = 1;
  pageSize = 10;
  total = 0;

  constructor(
    private roleService: RoleService,
    private router: Router,
    private modal: NzModalService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService.getAll(this.searchValue, this.pageIndex, this.pageSize).subscribe({
      next: (result) => {
        this.roles = result.items;
        this.total = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Failed to load roles');
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadRoles();
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadRoles();
  }

  createRole(): void {
    this.router.navigate(['/admin/roles/new']);
  }

  editRole(id: string): void {
    this.router.navigate([`/admin/roles/${id}/edit`]);
  }

  deleteRole(role: Role): void {
    if (role.isSystemRole) {
      this.message.warning('System roles cannot be deleted');
      return;
    }

    this.modal.confirm({
      nzTitle: 'Delete Role',
      nzContent: `Are you sure you want to delete the role "${role.name}"?`,
      nzOkText: 'Delete',
      nzOkDanger: true,
      nzOnOk: () => {
        this.roleService.delete(role.id).subscribe({
          next: () => {
            this.message.success('Role deleted successfully');
            this.loadRoles();
          },
          error: () => {
            this.message.error('Failed to delete role');
          }
        });
      }
    });
  }
}
