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
  styles: [`
    .dashboard-container {
        padding: 24px;
        min-height: calc(100vh - 64px);
    }

    .dashboard-header {
        margin-bottom: 32px;
        text-align: center;
    }

    .dashboard-header h2 {
        margin: 0 0 8px 0;
        font-size: 28px;
        font-weight: 600;
        color: rgba(0, 0, 0, 0.85);
    }

    .dashboard-subtitle {
        margin: 0;
        font-size: 16px;
        color: rgba(0, 0, 0, 0.65);
    }

    .stats-row {
        margin-bottom: 24px;
    }

    .stat-card {
        border-radius: 8px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        transition: transform 0.2s, box-shadow 0.2s;
    }

    .stat-card:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    }

    .stat-content {
        display: flex;
        align-items: center;
        gap: 16px;
    }

    .stat-icon {
        width: 48px;
        height: 48px;
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 24px;
        color: white;
    }

    .stat-icon.patients {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .stat-icon.tests {
        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    }

    .stat-icon.results {
        background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
    }

    .stat-icon.invoices {
        background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
    }

    .stat-info {
        flex: 1;
    }

    .stat-number {
        font-size: 24px;
        font-weight: 600;
        color: rgba(0, 0, 0, 0.85);
        line-height: 1;
    }

    .stat-label {
        font-size: 14px;
        color: rgba(0, 0, 0, 0.65);
        margin-top: 4px;
    }

    .actions-row {
        margin-bottom: 24px;
    }

    .action-card,
    .activity-card {
        border-radius: 8px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

    .action-buttons {
        display: flex;
        flex-direction: column;
        gap: 12px;
    }

    .action-btn {
        display: flex;
        align-items: center;
        justify-content: flex-start;
        gap: 8px;
        height: 48px;
        font-size: 14px;
    }

    .activity-placeholder {
        min-height: 200px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .empty-state {
        text-align: center;
        color: rgba(0, 0, 0, 0.45);
    }

    .empty-icon {
        font-size: 48px;
        margin-bottom: 16px;
        display: block;
    }

    .empty-state p {
        margin: 8px 0;
    }

    .empty-description {
        font-size: 12px;
        color: rgba(0, 0, 0, 0.25);
    }

    @media (max-width: 768px) {
        .dashboard-container {
            padding: 16px;
        }

        .dashboard-header h2 {
            font-size: 24px;
        }

        .stat-content {
            flex-direction: column;
            text-align: center;
            gap: 12px;
        }

        .action-btn {
            justify-content: center;
        }
    }
  `]
})
export class DashboardComponent {
    constructor() { }
}
