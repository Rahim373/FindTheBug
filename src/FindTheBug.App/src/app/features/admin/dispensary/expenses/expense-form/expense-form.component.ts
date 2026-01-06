import { Component, OnInit, inject } from '@angular/core';

import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { ExpenseService } from '../../../../../core/services/expense.service';
import { Expense, PaymentMethod } from '../../../../../core/models/expense.models';

@Component({
    selector: 'app-expense-form',
    standalone: true,
    imports: [
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzDatePickerModule,
    NzInputNumberModule,
    NzCardModule,
    NzPageHeaderModule
],
    templateUrl: './expense-form.component.html',
    styleUrls: ['./expense-form.component.css']
})
export class ExpenseFormComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly expenseService = inject(ExpenseService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly message = inject(NzMessageService);

    expenseForm!: FormGroup;
    isEditMode = false;
    expenseId: string | null = null;
    loading = false;
    paymentMethods = Object.values(PaymentMethod);
    department: string = '';

    ngOnInit(): void {
        this.initForm();

        // Get department from route data or query params
        this.department = this.route.snapshot.queryParamMap.get('department') || 'Dispensary';

        this.expenseId = this.route.snapshot.paramMap.get('id');
        if (this.expenseId) {
            this.isEditMode = true;
            this.loadExpense(this.expenseId);
        }
    }

    initForm(): void {
        this.expenseForm = this.fb.group({
            date: [new Date(), [Validators.required]],
            note: ['', [Validators.required]],
            amount: [0, [Validators.required, Validators.min(0)]],
            paymentMethod: [PaymentMethod.Cash, [Validators.required]],
            referenceNo: [''],
            attachment: ['']
        });
    }

    async loadExpense(id: string): Promise<void> {
        this.loading = true;
        try {
            const response = await this.expenseService.getExpense(id).toPromise();
            if (response?.isSuccess && response.data) {
                const expense = response.data;
                this.department = expense.department;
                this.expenseForm.patchValue({
                    date: new Date(expense.date),
                    note: expense.note,
                    amount: expense.amount,
                    paymentMethod: expense.paymentMethod,
                    referenceNo: expense.referenceNo || '',
                    attachment: expense.attachment || ''
                });
            }
        } catch (error) {
            console.error('Error loading expense:', error);
            this.message.error('Failed to load expense');
        } finally {
            this.loading = false;
        }
    }

    async onSubmit(): Promise<void> {
        if (this.expenseForm.valid) {
            this.loading = true;
            const formValue = this.expenseForm.value;

            const request = {
                id: this.expenseId || '',
                date: formValue.date,
                note: formValue.note,
                amount: formValue.amount,
                paymentMethod: formValue.paymentMethod,
                referenceNo: formValue.referenceNo || undefined,
                attachment: formValue.attachment || undefined,
                department: this.department
            };

            try {
                const operation = this.isEditMode
                    ? this.expenseService.updateExpense(request)
                    : this.expenseService.createExpense(request);

                const response = await operation.toPromise();
                if (response?.isSuccess) {
                    this.message.success(`Expense ${this.isEditMode ? 'updated' : 'created'} successfully`);
                    
                    // Navigate to the appropriate list based on department
                    const routePath = this.department === 'Lab' 
                        ? '/admin/transactions/expenses' 
                        : '/admin/dispensary/expenses';
                    
                    await this.router.navigate([routePath]);
                } else {
                    this.message.error(response?.message || 'Failed to save expense');
                }
            } catch (error) {
                console.error('Error saving expense:', error);
                this.message.error(`Failed to ${this.isEditMode ? 'update' : 'create'} expense`);
            } finally {
                this.loading = false;
            }
        } else {
            Object.values(this.expenseForm.controls).forEach(control => {
                control.markAsDirty();
                control.updateValueAndValidity();
            });
        }
    }

    cancel(): void {
        const routePath = this.department === 'Lab' 
            ? '/admin/transactions/expenses' 
            : '/admin/dispensary/expenses';
        
        this.router.navigate([routePath]);
    }
}