import { Component, AfterViewInit, viewChild } from '@angular/core';

import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../shared/sidebar/sidebar.component';
import { HeaderComponent } from '../shared/header/header.component';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [
    NzLayoutModule,
    RouterOutlet,
    SidebarComponent,
    HeaderComponent
],
    template: `
    <nz-layout class="admin-layout">
        <app-sidebar></app-sidebar>
        <nz-layout class="main-layout">
            <app-header (toggleSidebar)="onToggleSidebar()"></app-header>
            <nz-content class="content">
                <router-outlet></router-outlet>
            </nz-content>
        </nz-layout>
    </nz-layout>
  `,
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent implements AfterViewInit {
    readonly sidebarComponent = viewChild(SidebarComponent);
    readonly headerComponent = viewChild(HeaderComponent);

    ngAfterViewInit(): void {
        this.updateLayoutClasses();
    }

    onToggleSidebar(): void {
        const sidebar = this.sidebarComponent();
        if (sidebar) {
            sidebar.toggleCollapse();
            this.updateLayoutClasses();
        }
    }

    private updateLayoutClasses(): void {
        const mainLayout = document.querySelector('.main-layout');
        const header = document.querySelector('.header');
        const sidebar = this.sidebarComponent();
        
        if (sidebar?.isCollapsed) {
            mainLayout?.classList.add('collapsed');
            header?.classList.add('collapsed');
        } else {
            mainLayout?.classList.remove('collapsed');
            header?.classList.remove('collapsed');
        }
    }
}