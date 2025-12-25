import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
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
            <li nz-submenu nzTitle="User Management" nzIcon="team" [nzOpen]="isUserManagementOpen">
                <ul>
                    <li nz-menu-item nzMatchRouter>
                        <a routerLink="/admin/users">
                            <span nz-icon nzType="user"></span>
                            <span>Users</span>
                        </a>
                    </li>
                    <li nz-menu-item nzMatchRouter>
                        <a routerLink="/admin/roles">
                             <span nzTheme="fill" nz-icon nzType="security-scan"></span>
                            <span>Roles</span>
                        </a>
                    </li>
                </ul>
            </li>
            <li nz-submenu nzTitle="Medical Management" nzIcon="fa:stethoscope" [nzOpen]="isMedicalManagementOpen">
                <ul>
                    <li nz-menu-item nzMatchRouter>
                        <a routerLink="/admin/doctors">
                            <span nz-icon nzType="fa:user-doctor"></span>
                            <span>Doctors</span>
                        </a>
                    </li>
                    <li nz-menu-item>
                        <a routerLink="/admin/patients">
                            <span nz-icon nzType="fa:hospital-user"></span>
                            <span>Patients</span>
                        </a>
                    </li>
                </ul>
            </li>
            <li nz-submenu nzTitle="Dispensary" nzIcon="fa:pills" [nzOpen]="isDispensaryOpen">
                <ul>
                    <li nz-menu-item nzMatchRouter>
                        <a routerLink="/admin/dispensary/drugs">
                            <span nz-icon nzType="fa:syringe"></span>
                            <span>Drugs</span>
                        </a>
                    </li>
                    <li nz-menu-item nzMatchRouter>
                        <a routerLink="/admin/dispensary/products">
                            <span nz-icon nzType="dropbox"></span>
                            <span>Products</span>
                        </a>
                    </li>
                </ul>
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
            <li nz-submenu nzTitle="Settings" nzIcon="setting" [nzOpen]="isSettingsOpen">
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
export class SidebarComponent implements OnInit, OnDestroy {
    isCollapsed = false;
    private router = inject(Router);
    private routerSubscription: any;
    
    // Submenu expansion states
    isUserManagementOpen = false;
    isMedicalManagementOpen = false;
    isDispensaryOpen = false;
    isSettingsOpen = false;

    ngOnInit(): void {
        this.updateExpandedMenus();
        
        // Subscribe to router changes to update expanded menus
        this.routerSubscription = this.router.events.pipe(
            filter(event => event instanceof NavigationEnd)
        ).subscribe(() => {
            this.updateExpandedMenus();
        });
    }

    ngOnDestroy(): void {
        if (this.routerSubscription) {
            this.routerSubscription.unsubscribe();
        }
    }

    private updateExpandedMenus(): void {
        const url = this.router.url;
        
        // Check if current URL matches any submenu items
        this.isUserManagementOpen = this.isRouteInSubmenu(url, ['/admin/users', '/admin/roles']);
        this.isMedicalManagementOpen = this.isRouteInSubmenu(url, ['/admin/doctors', '/admin/patients']);
        this.isDispensaryOpen = this.isRouteInSubmenu(url, ['/admin/dispensary/drugs', '/admin/dispensary/products']);
        this.isSettingsOpen = this.isRouteInSubmenu(url, ['/admin/settings/profile', '/admin/settings/system']);
    }

    private isRouteInSubmenu(currentUrl: string, menuRoutes: string[]): boolean {
        return menuRoutes.some(route => currentUrl.startsWith(route));
    }

    toggleCollapse(): void {
        this.isCollapsed = !this.isCollapsed;
    }
}