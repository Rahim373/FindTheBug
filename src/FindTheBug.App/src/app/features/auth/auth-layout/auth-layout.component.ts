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
            <h1>Al Razi Diagnostics Center and Consultaion</h1>
            <p>Diagnostics Lab Management System</p>
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
  styles: [`
    .auth-container {
      min-height: 100vh;
      background-color: #f0f2f5;
      background-image: url('https://gw.alipayobjects.com/zos/rmsportal/TVYTbAXWheQpRcWDaDMu.svg');
      background-repeat: no-repeat;
      background-position: center 110px;
      background-size: 100%;
    }

    .auth-row {
      min-height: 100vh;
      padding: 24px;
    }

    .logo-container {
      text-align: center;
      margin-bottom: 40px;
    }

    .logo-container h1 {
      font-size: 33px;
      color: rgba(0, 0, 0, 0.85);
      font-family: Avenir, 'Helvetica Neue', Arial, Helvetica, sans-serif;
      font-weight: 600;
      margin-bottom: 12px;
    }

    .logo-container p {
      color: rgba(0, 0, 0, 0.45);
      font-size: 14px;
      margin-bottom: 0;
    }

    .auth-card {
      box-shadow: 0 1px 2px -2px rgba(0, 0, 0, 0.16), 0 3px 6px 0 rgba(0, 0, 0, 0.12), 0 5px 12px 4px rgba(0, 0, 0, 0.09);
      border-radius: 4px;
    }

    .footer {
      margin-top: 24px;
      text-align: center;
    }

    .footer p {
      color: rgba(0, 0, 0, 0.45);
      font-size: 14px;
    }
  `]
})
export class AuthLayoutComponent { }
