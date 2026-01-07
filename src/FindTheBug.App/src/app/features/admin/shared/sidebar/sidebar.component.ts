import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, NavigationEnd } from '@angular/router';

import { filter } from 'rxjs/operators';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { PermissionService, PermissionType } from '../../../../core/services/permission.service';
import {NgOptimizedImage} from '@angular/common';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [
    RouterLink,
    NzMenuModule,
    NzIconModule,
    NzLayoutModule,
    NgOptimizedImage
],
    template: `
    <nz-sider
      [nzCollapsed]="isCollapsed"
      [nzWidth]="200"
      [nzCollapsedWidth]="80"
      nzTheme="light"
      class="sidebar">
      <div class="logo">
        @if (!isCollapsed) {
          <img ngSrc="/images/Logo.svg" loading="eager" height="50" width="89" />
          <!-- <span class="logo-text">FindTheBug</span> -->
        }
        @if (isCollapsed) {
          <span class="logo-icon">FTB</span>
        }
      </div>
    
      <ul nz-menu [nzMode]="isCollapsed ? 'vertical' : 'inline'" [nzInlineCollapsed]="isCollapsed">
        <li nz-menu-item nzMatchRouter>
          <a routerLink="/admin/dashboard">
            <span nz-icon nzType="dashboard"></span>
            <span>Dashboard</span>
          </a>
        </li>
        @if (permissionService.hasAnyPermissionSync('UserManagement')) {
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
        }
        @if (permissionService.hasAnyPermissionSync('DoctorManagement') || permissionService.hasAnyPermissionSync('Patients')) {
          <li nz-submenu nzTitle="Medical Management" nzIcon="fa:stethoscope" [nzOpen]="isMedicalManagementOpen"
            >
            <ul>
              @if (permissionService.hasPermissionSync('DoctorManagement', PermissionType.View)) {
                <li nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/doctors">
                    <span nz-icon nzType="fa:user-doctor"></span>
                    <span>Doctors</span>
                  </a>
                </li>
              }
              @if (permissionService.hasPermissionSync('Patients', PermissionType.View)) {
                <li nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/patients">
                    <span nz-icon nzType="fa:hospital-user"></span>
                    <span>Patients</span>
                  </a>
                </li>
              }
            </ul>
          </li>
        }
        @if (permissionService.hasAnyPermissionSync('Dispensary')) {
          <li nz-submenu nzTitle="Dispensary" nzIcon="fa:pills" [nzOpen]="isDispensaryOpen"
            >
            <ul>
              @if (permissionService.hasPermissionSync('Dispensary', PermissionType.View)) {
                <li nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/dispensary/drugs">
                    <span nz-icon nzType="fa:syringe"></span>
                    <span>Drugs</span>
                  </a>
                </li>
              }
              @if (permissionService.hasPermissionSync('Dispensary', PermissionType.View)) {
                <li nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/dispensary/products">
                    <span nz-icon nzType="dropbox"></span>
                    <span>Products</span>
                  </a>
                </li>
              }
              <!-- @if (permissionService.hasPermissionSync('Dispensary', PermissionType.View)) {
                <li nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/dispensary/expenses">
                    <span nz-icon nzType="money-collect"></span>
                    <span>Expenses</span>
                  </a>
                </li>
              } -->
            </ul>
          </li>
        }
        @if (permissionService.hasAnyPermissionSync('Reception')) {
          <li nz-submenu nzTitle="Reception" nzIcon="fa:users-line" [nzOpen]="isReceptionOpen"
            >
            <ul>
              @if (permissionService.hasPermissionSync('Reception', PermissionType.View)) {
                <li nzType="fa:file-invoice" nz-menu-item nzMatchRouter>
                  <a routerLink="/admin/receipts">
                    <span nz-icon nzType="fa:file-invoice"></span>
                    <span>Receipts</span>
                  </a>
                </li>
              }
            </ul>
          </li>
        }
        <!-- @if (permissionService.hasAnyPermissionSync('Accounts')) {
          <li nz-submenu nzTitle="Transactions" nzIcon="transaction" [nzOpen]="isTransactionsOpen"
            >
            <ul>
              <li nz-menu-item nzMatchRouter>
                <a routerLink="/admin/transactions/expenses">
                  <span nz-icon nzType="file-invoice-dollar"></span>
                  <span>Lab Expenses</span>
                </a>
              </li>
            </ul>
          </li>
        } -->
      </ul>
    </nz-sider>
    `,
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit, OnDestroy {
    isCollapsed = false;
    private router = inject(Router);
    readonly permissionService = inject(PermissionService);
    readonly PermissionType = PermissionType;
    private routerSubscription: any;
    
    // Submenu expansion states
    isUserManagementOpen = false;
    isMedicalManagementOpen = false;
    isDispensaryOpen = false;
    isReceptionOpen = false;
    isTransactionsOpen = false;
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
        this.isDispensaryOpen = this.isRouteInSubmenu(url, ['/admin/dispensary/drugs', '/admin/dispensary/products', '/admin/dispensary/expenses']);
        this.isReceptionOpen = this.isRouteInSubmenu(url, ['/admin/receipts']);
        this.isTransactionsOpen = this.isRouteInSubmenu(url, ['/admin/transactions/expenses']);
        this.isSettingsOpen = this.isRouteInSubmenu(url, ['/admin/settings/profile', '/admin/settings/system']);
    }

    private isRouteInSubmenu(currentUrl: string, menuRoutes: string[]): boolean {
        return menuRoutes.some(route => currentUrl.startsWith(route));
    }

    toggleCollapse(): void {
        this.isCollapsed = !this.isCollapsed;
    }
}