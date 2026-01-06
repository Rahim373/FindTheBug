import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RoleService } from '../../../../core/services/role.service';
import { Role } from '../../../../core/models/role.models';
import { PagedResult } from '../../../../core/models/common.models';

@Component({
    selector: 'app-roles-list',
    standalone: true,
    imports: [
    FormsModule,
    NzTableModule,
    NzButtonModule,
    NzInputModule,
    NzIconModule,
    NzModalModule,
    NzTagModule,
    NzSpaceModule,
    NzCardModule
],
    templateUrl: './roles-list.component.html',
    styleUrl: './roles-list.component.css'
})
export class RolesListComponent implements OnInit {
    roles: Role[] = [];
    loading = false;
    searchText = '';
    pageIndex = 1;
    pageSize = 10;
    total = 0;

    constructor(
        private roleService: RoleService,
        private router: Router,
        private message: NzMessageService,
        private modal: NzModalService
    ) { }

    ngOnInit(): void {
        this.loadRoles();
    }

    async loadRoles(): Promise<void> {
        this.loading = true;
        try {
            const result: PagedResult<Role> = await this.roleService.getRolesAsync(
                this.searchText || undefined,
                this.pageIndex,
                this.pageSize
            );
            this.roles = result.items;
            this.total = result.totalCount;
        } catch (error) {
            console.error('Error loading roles:', error);
            this.message.error('Failed to load roles. Please try again.');
        } finally {
            this.loading = false;
        }
    }

    onSearch(): void {
        this.pageIndex = 1;
        this.loadRoles();
    }

    onPageChange(pageIndex: number): void {
        this.pageIndex = pageIndex;
        this.loadRoles();
    }

    onPageSizeChange(pageSize: number): void {
        this.pageSize = pageSize;
        this.pageIndex = 1;
        this.loadRoles();
    }

    createRole(): void {
        this.router.navigate(['/admin/roles/create']);
    }

    editRole(id: string): void {
        this.router.navigate(['/admin/roles', id, 'edit']);
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
            nzOnOk: async () => {
                try {
                    await this.roleService.deleteRoleAsync(role.id);
                    this.message.success('Role deleted successfully');
                    await this.loadRoles();
                } catch (error) {
                    console.error('Error deleting role:', error);
                    this.message.error('Failed to delete role. Please try again.');
                }
            }
        });
    }
}
