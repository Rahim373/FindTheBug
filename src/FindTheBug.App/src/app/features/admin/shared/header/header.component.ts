import { Component, inject, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [
        CommonModule,
        NzLayoutModule,
        NzButtonModule,
        NzIconModule,
        NzDropDownModule,
        NzAvatarModule,
        NzMenuModule
    ],
    template: `
    <nz-header class="header">
        <div class="header-left">
            <button 
                nz-button 
                nzType="text" 
                nzShape="circle"
                (click)="toggleSidebar.emit()">
                <span nz-icon nzType="menu" nzTheme="outline"></span>
            </button>
            <h1 class="page-title">{{ pageTitle }}</h1>
        </div>
        
        <div class="header-right">
            <div nz-dropdown [nzDropdownMenu]="userMenu" nzPlacement="bottomRight">
                <button nz-button nzType="text" class="user-menu">
                    <nz-avatar nzIcon="user" nzSize="small"></nz-avatar>
                    <span class="user-name">Admin User</span>
                    <span nz-icon nzType="down"></span>
                </button>
            </div>
            <nz-dropdown-menu #userMenu="nzDropdownMenu">
                <ul nz-menu>
                    <li nz-menu-item (click)="onProfile()">
                        <span nz-icon nzType="user"></span>
                        Profile
                    </li>
                    <li nz-menu-item (click)="onSettings()">
                        <span nz-icon nzType="setting"></span>
                        Settings
                    </li>
                    <li nz-menu-divider></li>
                    <li nz-menu-item (click)="onLogout()">
                        <span nz-icon nzType="logout"></span>
                        Logout
                    </li>
                </ul>
            </nz-dropdown-menu>
        </div>
    </nz-header>
  `,
  styles: [`
    .header {
        position: fixed;
        top: 0;
        right: 0;
        left: 200px;
        z-index: 999;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 24px;
        background: #fff;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        transition: left 0.2s;
    }

    .header.collapsed {
        left: 80px;
    }

    .header-left {
        display: flex;
        align-items: center;
        gap: 16px;
    }

    .page-title {
        margin: 0;
        font-size: 18px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.85);
    }

    .header-right {
        display: flex;
        align-items: center;
    }

    .user-menu {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 4px 8px;
        border-radius: 6px;
        transition: background-color 0.2s;
    }

    .user-menu:hover {
        background-color: rgba(0, 0, 0, 0.06);
    }

    .user-name {
        font-size: 14px;
        color: rgba(0, 0, 0, 0.85);
    }

    @media (max-width: 768px) {
        .header {
            left: 0;
            padding: 0 16px;
        }

        .header.collapsed {
            left: 0;
        }

        .user-name {
            display: none;
        }

        .page-title {
            font-size: 16px;
        }
    }
  `]
})
export class HeaderComponent {
    @Output() toggleSidebar = new EventEmitter<void>();
    pageTitle = 'Dashboard';
    
    private authService = inject(AuthService);

    onProfile(): void {
        // Navigate to profile page
        console.log('Navigate to profile');
    }

    onSettings(): void {
        // Navigate to settings page
        console.log('Navigate to settings');
    }

    async onLogout(): Promise<void> {
        await this.authService.logout();
    }

    setPageTitle(title: string): void {
        this.pageTitle = title;
    }
}
