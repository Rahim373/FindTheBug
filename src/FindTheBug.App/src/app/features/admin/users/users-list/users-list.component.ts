import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { UserService } from '../../../../core/services/user.service';
import { User } from '../../../../core/models/user.models';

@Component({
    selector: 'app-users-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        NzTableModule,
        NzButtonModule,
        NzInputModule,
        NzIconModule,
        NzModalModule,
        NzTagModule
    ],
    templateUrl: './users-list.component.html',
    styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
    private readonly userService = inject(UserService);
    private readonly router = inject(Router);
    private readonly modal = inject(NzModalService);
    private readonly message = inject(NzMessageService);

    users: User[] = [];
    loading = false;
    searchText = '';
    
    // Pagination
    pageNumber = 1;
    pageSize = 10;
    totalCount = 0;

    ngOnInit(): void {
        this.loadUsers();
    }

    loadUsers(): void {
        this.loading = true;
        this.userService.getUsers(this.pageNumber, this.pageSize, this.searchText)
            .subscribe({
                next: (response) => {
                    if (response.isSuccess && response.data) {
                        this.users = response.data.items;
                        this.totalCount = response.data.totalCount;
                        this.pageNumber = response.data.pageNumber;
                        this.pageSize = response.data.pageSize;
                    }
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error loading users:', error);
                    this.message.error('Failed to load users');
                    this.loading = false;
                }
            });
    }

    onSearch(): void {
        this.pageNumber = 1;
        this.loadUsers();
    }

    onPageChange(pageNumber: number): void {
        this.pageNumber = pageNumber;
        this.loadUsers();
    }

    onPageSizeChange(pageSize: number): void {
        this.pageSize = pageSize;
        this.pageNumber = 1;
        this.loadUsers();
    }

    createUser(): void {
        this.router.navigate(['/admin/users/new']);
    }

    viewUser(id: string): void {
        this.router.navigate(['/admin/users', id]);
    }

    editUser(id: string): void {
        this.router.navigate(['/admin/users', id, 'edit']);
    }

    deleteUser(user: User): void {
        this.modal.confirm({
            nzTitle: 'Delete User',
            nzContent: `Are you sure you want to delete ${user.firstName} ${user.lastName}?`,
            nzOkText: 'Delete',
            nzOkDanger: true,
            nzOnOk: () => {
                this.userService.deleteUser(user.id).subscribe({
                    next: (response) => {
                        if (response.isSuccess) {
                            this.message.success('User deleted successfully');
                            this.loadUsers();
                        } else {
                            this.message.error(response.errors?.[0]?.description || 'Failed to delete user');
                        }
                    },
                    error: (error) => {
                        console.error('Error deleting user:', error);
                        this.message.error('Failed to delete user');
                    }
                });
            }
        });
    }
}
