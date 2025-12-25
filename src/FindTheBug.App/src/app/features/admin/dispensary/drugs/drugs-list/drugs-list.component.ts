import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { DrugService } from '../../../../../core/services/drug.service';
import { DrugListItem } from '../../../../../core/models/drug.models';

@Component({
    selector: 'app-drugs-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        NzTableModule,
        NzButtonModule,
        NzInputModule,
        NzIconModule,
        NzModalModule,
        NzTagModule
    ],
    templateUrl: './drugs-list.component.html',
    styleUrls: ['./drugs-list.component.css']
})
export class DrugsListComponent implements OnInit {
    private readonly drugService = inject(DrugService);
    private readonly router = inject(Router);
    private readonly modal = inject(NzModalService);
    private readonly message = inject(NzMessageService);

    drugs: DrugListItem[] = [];
    loading = false;
    searchText = '';

    // Pagination
    pageNumber = 1;
    pageSize = 10;
    totalCount = 0;

    ngOnInit(): void {
        this.loadDrugs();
    }

    async loadDrugs(): Promise<void> {
        this.loading = true;
        try {
            const response = await this.drugService.getDrugs(this.searchText || undefined, this.pageNumber, this.pageSize).toPromise();
            if (response?.isSuccess && response.data) {
                this.drugs = response.data.items || [];
                this.totalCount = response.data.totalCount || 0;
            }
        } catch (error) {
            console.error('Error loading drugs:', error);
            this.message.error('Failed to load drugs');
        } finally {
            this.loading = false;
        }
    }
    onSearch(): void {
        this.pageNumber = 1;
        this.loadDrugs();
    }

    onPageChange(pageNumber: number): void {
        this.pageNumber = pageNumber;
        this.loadDrugs();
    }

    onPageSizeChange(pageSize: number): void {
        this.pageSize = pageSize;
        this.pageNumber = 1;
        this.loadDrugs();
    }

    createDrug(): void {
        this.router.navigate(['/admin/dispensary/drugs/create']);
    }

    editDrug(id: string): void {
        this.router.navigate(['/admin/dispensary/drugs', id, 'edit']);
    }

    deleteDrug(drug: DrugListItem): void {
        this.modal.confirm({
            nzTitle: 'Delete Drug',
            nzContent: `Are you sure you want to delete ${drug.name}?`,
            nzOkText: 'Delete',
            nzOkDanger: true,
            nzOnOk: async () => {
                try {
                    await this.drugService.deleteDrug(drug.id).toPromise();
                    this.message.success('Drug deleted successfully');
                    await this.loadDrugs();
                } catch (error) {
                    console.error('Error deleting drug:', error);
                    this.message.error('Failed to delete drug');
                }
            }
        });
    }
}
