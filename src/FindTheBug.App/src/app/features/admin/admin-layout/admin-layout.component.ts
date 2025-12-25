import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../shared/sidebar/sidebar.component';
import { HeaderComponent } from '../shared/header/header.component';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [
        CommonModule,
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
    @ViewChild(SidebarComponent) sidebarComponent!: SidebarComponent;
    @ViewChild(HeaderComponent) headerComponent!: HeaderComponent;

    ngAfterViewInit(): void {
        this.updateLayoutClasses();
    }

    onToggleSidebar(): void {
        if (this.sidebarComponent) {
            this.sidebarComponent.toggleCollapse();
            this.updateLayoutClasses();
        }
    }

    private updateLayoutClasses(): void {
        const mainLayout = document.querySelector('.main-layout');
        const header = document.querySelector('.header');
        
        if (this.sidebarComponent?.isCollapsed) {
            mainLayout?.classList.add('collapsed');
            header?.classList.add('collapsed');
        } else {
            mainLayout?.classList.remove('collapsed');
            header?.classList.remove('collapsed');
        }
    }
}