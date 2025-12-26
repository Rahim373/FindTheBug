import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { ExpenseService } from '../../../../../core/services/expense.service';
import { ExpenseListItem } from '../../../../../core/models/expense.models';

@Component({
    selector: 'app-expenses-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        NzTableModule,
        NzButtonModule,
        NzInputModule,
        NzIconModule,
        NzModalModule,
        NzTagModule,
        NzDatePickerModule
    ],
    templateUrl: './expenses-list.component.html',
    styleUrls: ['./expenses-list.component.css']
})
export class ExpensesListComponent implements OnInit {
    private readonly expenseService = inject(ExpenseService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly modal = inject(NzModalService);
    private readonly message = inject(NzMessageService);

    expenses: ExpenseListItem[] = [];
    loading = false;
    searchText = '';
    department = '';
    title = '';

    // Pagination
    pageNumber = 1;
    pageSize = 10;
    totalCount = 0;

    ngOnInit(): void {
        // Get department from route
        this.department = this.route.snapshot.data['department'] || 'Dispensary';
        this.title = this.department === 'Lab' ? 'Lab Expenses' : 'Dispensary Expenses';
        this.loadExpenses();
    }

    async loadExpenses(): Promise<void> {
        this.loading = true;
        try {
            const response = await this.expenseService.getExpenses(
                this.department,
                this.searchText || undefined,
                this.pageNumber,
                this.pageSize
            ).toPromise();
            
            if (response?.isSuccess && response.data) {
                this.expenses = response.data.items || [];
                this.totalCount = response.data.totalCount || 0;
            }
        } catch (error) {
            console.error('Error loading expenses:', error);
            this.message.error('Failed to load expenses');
        } finally {
            this.loading = false;
        }
    }

    onSearch(): void {
        this.pageNumber = 1;
        this.loadExpenses();
    }

    onPageChange(pageNumber: number): void {
        this.pageNumber = pageNumber;
        this.loadExpenses();
    }

    onPageSizeChange(pageSize: number): void {
        this.pageSize = pageSize;
        this.pageNumber = 1;
        this.loadExpenses();
    }

    createExpense(): void {
        const createRoute = this.department === 'Lab'
            ? '/admin/transactions/expenses/create'
            : '/admin/dispensary/expenses/create';
        
        this.router.navigate([createRoute], { queryParams: { department: this.department } });
    }

    editExpense(id: string): void {
        const editRoute = this.department === 'Lab'
            ? '/admin/transactions/expenses'
            : '/admin/dispensary/expenses';
        
        this.router.navigate([editRoute, id, 'edit'], { queryParams: { department: this.department } });
    }

    deleteExpense(expense: ExpenseListItem): void {
        this.modal.confirm({
            nzTitle: 'Delete Expense',
            nzContent: `Are you sure you want to delete this expense of ${expense.amount}?`,
            nzOkText: 'Delete',
            nzOkDanger: true,
            nzOnOk: async () => {
                try {
                    const response = await this.expenseService.deleteExpense(expense.id).toPromise();
                    if (response?.isSuccess) {
                        this.message.success('Expense deleted successfully');
                        await this.loadExpenses();
                    } else {
                        this.message.error(response?.message || 'Failed to delete expense');
                    }
                } catch (error) {
                    console.error('Error deleting expense:', error);
                    this.message.error('Failed to delete expense');
                }
            }
        });
    }

    formatDate(date: string): string {
        return new Date(date).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }

    getPaymentMethodColor(paymentMethod: string): string {
        switch (paymentMethod) {
            case 'Cash':
                return 'green';
            case 'Cheque':
                return 'blue';
            case 'bKash':
                return 'magenta';
            case 'Nagad':
                return 'orange';
            default:
                return 'default';
        }
    }
}