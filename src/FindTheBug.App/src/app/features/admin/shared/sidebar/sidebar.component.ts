import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        NzMenuModule,
        NzIconModule,
        NzLayoutModule
    ],
    template: `
    <nz-sider 
        [nzCollapsed]="isCollapsed" 
        [nzWidth]="200" 
        [nzCollapsedWidth]="80"
        class="sidebar">
        <div class="logo">
            <ng-container *ngIf="!isCollapsed">
                <span class="logo-text">FindTheBug</span>
            </ng-container>
            <ng-container *ngIf="isCollapsed">
                <span class="logo-icon">FTB</span>
            </ng-container>
        </div>
        
        <ul nz-menu nzTheme="dark" [nzMode]="isCollapsed ? 'vertical' : 'inline'" [nzInlineCollapsed]="isCollapsed">
            <li nz-menu-item nzMatchRouter>
                <a routerLink="/admin/dashboard">
                    <span nz-icon nzType="dashboard"></span>
                    <span>Dashboard</span>
                </a>
            </li>
            <li nz-menu-item nzMatchRouter>
                <a routerLink="/admin/users">
                    <span nz-icon nzType="team"></span>
                    <span>Users</span>
                </a>
            </li>
            <li nz-menu-item>
                <a routerLink="/admin/patients">
                    <span nz-icon nzType="user"></span>
                    <span>Patients</span>
                </a>
            </li>
            <li nz-menu-item>
                <a routerLink="/admin/tests">
                    <span nz-icon nzType="experiment"></span>
                    <span>Diagnostic Tests</span>
                </a>
            </li>
            <li nz-menu-item>
                <a routerLink="/admin/results">
                    <span nz-icon nzType="file-text"></span>
                    <span>Test Results</span>
                </a>
            </li>
            <li nz-menu-item>
                <a routerLink="/admin/invoices">
                    <span nz-icon nzType="file-pdf"></span>
                    <span>Invoices</span>
                </a>
            </li>
            <li nz-submenu nzTitle="Settings" nzIcon="setting">
                <ul>
                    <li nz-menu-item>
                        <a routerLink="/admin/settings/profile">
                            <span>Profile</span>
                        </a>
                    </li>
                    <li nz-menu-item>
                        <a routerLink="/admin/settings/system">
                            <span>System</span>
                        </a>
                    </li>
                </ul>
            </li>
        </ul>
    </nz-sider>
  `,
  styles: [`
    .sidebar {
        position: fixed;
        left: 0;
        top: 0;
        bottom: 0;
        overflow-y: auto;
        z-index: 1000;
    }

    .logo {
        height: 64px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: rgba(255, 255, 255, 0.1);
        margin: 16px;
        border-radius: 6px;
        color: white;
        font-weight: bold;
        font-size: 18px;
    }

    .logo-text {
        font-size: 18px;
        font-weight: 600;
    }

    .logo-icon {
        font-size: 16px;
        font-weight: 700;
    }

    :host ::ng-deep .ant-menu-dark {
        background: transparent;
    }

    :host ::ng-deep .ant-menu-dark .ant-menu-item-selected {
        background-color: #1890ff;
    }

    :host ::ng-deep .ant-menu-item a {
        color: rgba(255, 255, 255, 0.85);
        text-decoration: none;
    }

    :host ::ng-deep .ant-menu-item a:hover {
        color: #1890ff;
    }

    :host ::ng-deep .ant-menu-item-selected a {
        color: #1890ff;
    }
  `]
})
export class SidebarComponent {
    isCollapsed = false;
    private router = inject(Router);

    toggleCollapse(): void {
        this.isCollapsed = !this.isCollapsed;
    }
}
