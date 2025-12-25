import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzRowDirective, NzColDirective } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        NzCardModule,
        NzRowDirective,
        NzColDirective,
        NzIconModule,
        NzButtonModule
    ],
    template: `
    <div class="dashboard-container">
        <div class="dashboard-header">
            <h2>Welcome to FindTheBug Admin Dashboard</h2>
            <p class="dashboard-subtitle">Manage your diagnostic testing system efficiently</p>
        </div>

        <nz-row [nzGutter]="24" class="stats-row">
            <nz-col [nzXs]="24" [nzSm]="12" [nzMd]="6">
                <nz-card class="stat-card">
                    <div class="stat-content">
                        <div class="stat-icon patients">
                            <span nz-icon nzType="user" nzTheme="outline"></span>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">0</div>
                            <div class="stat-label">Total Patients</div>
                        </div>
                    </div>
                </nz-card>
            </nz-col>

            <nz-col [nzXs]="24" [nzSm]="12" [nzMd]="6">
                <nz-card class="stat-card">
                    <div class="stat-content">
                        <div class="stat-icon tests">
                            <span nz-icon nzType="experiment" nzTheme="outline"></span>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">0</div>
                            <div class="stat-label">Diagnostic Tests</div>
                        </div>
                    </div>
                </nz-card>
            </nz-col>

            <nz-col [nzXs]="24" [nzSm]="12" [nzMd]="6">
                <nz-card class="stat-card">
                    <div class="stat-content">
                        <div class="stat-icon results">
                            <span nz-icon nzType="file-text" nzTheme="outline"></span>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">0</div>
                            <div class="stat-label">Test Results</div>
                        </div>
                    </div>
                </nz-card>
            </nz-col>

            <nz-col [nzXs]="24" [nzSm]="12" [nzMd]="6">
                <nz-card class="stat-card">
                    <div class="stat-content">
                        <div class="stat-icon invoices">
                            <span nz-icon nzType="file-pdf" nzTheme="outline"></span>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">0</div>
                            <div class="stat-label">Invoices</div>
                        </div>
                    </div>
                </nz-card>
            </nz-col>
        </nz-row>

        <nz-row [nzGutter]="24" class="actions-row">
            <nz-col [nzXs]="24" [nzMd]="12">
                <nz-card title="Quick Actions" class="action-card">
                    <div class="action-buttons">
                        <button nz-button nzType="primary" nzSize="large" class="action-btn">
                            <span nz-icon nzType="plus"></span>
                            New Patient
                        </button>
                        <button nz-button nzType="default" nzSize="large" class="action-btn">
                            <span nz-icon nzType="plus"></span>
                            New Test Entry
                        </button>
                        <button nz-button nzType="default" nzSize="large" class="action-btn">
                            <span nz-icon nzType="file-pdf"></span>
                            Generate Invoice
                        </button>
                    </div>
                </nz-card>
            </nz-col>

            <nz-col [nzXs]="24" [nzMd]="12">
                <nz-card title="Recent Activity" class="activity-card">
                    <div class="activity-placeholder">
                        <div class="empty-state">
                            <span nz-icon nzType="inbox" nzTheme="outline" class="empty-icon"></span>
                            <p>No recent activity</p>
                            <p class="empty-description">Start by adding patients or test entries to see activity here.</p>
                        </div>
                    </div>
                </nz-card>
            </nz-col>
        </nz-row>
    </div>
  `,
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
    constructor() { }
}