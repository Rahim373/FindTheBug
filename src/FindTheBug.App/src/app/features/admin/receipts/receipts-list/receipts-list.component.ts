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
import { ReceiptService } from '../../../../core/services/receipt.service';
import { ReceiptListItem } from '../../../../core/models/receipt.models';

@Component({
  selector: 'app-receipts-list',
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
  templateUrl: './receipts-list.component.html',
  styleUrls: ['./receipts-list.component.css']
})
export class ReceiptsListComponent implements OnInit {
  private readonly receiptService = inject(ReceiptService);
  private readonly router = inject(Router);
  private readonly modal = inject(NzModalService);
  private readonly message = inject(NzMessageService);

  receipts: ReceiptListItem[] = [];
  loading = false;
  searchText = '';
  
  // Pagination
  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit(): void {
    this.loadReceipts();
  }

  loadReceipts(): void {
    this.loading = true;
    this.receiptService.getReceipts(this.searchText || undefined, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        if (response) {
          this.receipts = response.items || [];
          this.totalCount = response.totalCount || 0;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading receipts:', error);
        this.message.error('Failed to load receipts');
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadReceipts();
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadReceipts();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageNumber = 1;
    this.loadReceipts();
  }

  createReceipt(): void {
    this.router.navigate(['/admin/receipts/create']);
  }

  viewReceipt(id: string): void {
    this.router.navigate(['/admin/receipts', id]);
  }

  editReceipt(id: string): void {
    this.router.navigate(['/admin/receipts', id, 'edit']);
  }

  deleteReceipt(receipt: ReceiptListItem): void {
    this.modal.confirm({
      nzTitle: 'Delete Receipt',
      nzContent: `Are you sure you want to delete receipt ${receipt.invoiceNumber} for ${receipt.fullName}?`,
      nzOkText: 'Delete',
      nzOkDanger: true,
      nzOnOk: () => {
        this.receiptService.deleteReceipt(receipt.id).subscribe({
          next: () => {
            this.message.success('Receipt deleted successfully');
            this.loadReceipts();
          },
          error: (error) => {
            console.error('Error deleting receipt:', error);
            this.message.error('Failed to delete receipt');
          }
        });
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'paid':
        return 'success';
      case 'due':
        return 'warning';
      case 'pending':
        return 'processing';
      case 'void':
        return 'error';
      default:
        return 'default';
    }
  }
}