import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzLayoutModule } from 'ng-zorro-antd/layout';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    NzGridModule,
    NzCardModule,
    NzLayoutModule
  ],
  template: `
    <div class="auth-container">
      <div nz-row nzJustify="center" nzAlign="middle" class="auth-row">
        <div nz-col [nzXs]="24" [nzSm]="16" [nzMd]="12" [nzLg]="8" [nzXl]="6">
          <div class="logo-container">
            <h1>Mathbaria Al-Razi Diagnostics & Consultaion Center</h1>
          </div>
          <nz-card [nzBordered]="false" class="auth-card">
            <router-outlet></router-outlet>
          </nz-card>
          <div class="footer">
            <p>© 2025 FindTheBug. All rights reserved.</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrl: './auth-layout.component.css'
})
export class AuthLayoutComponent { }