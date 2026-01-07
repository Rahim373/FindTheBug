import { Component, inject, OnInit } from '@angular/core';
import { ChangeDetectionStrategy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { computed, signal } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';

import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzGridModule } from 'ng-zorro-antd/grid';

import { ReceiptService } from '../../../../core/services/receipt.service';
import { DiagnosticTestService } from '../../../../core/services/diagnostic-test.service';
import { DoctorService } from '../../../../core/services/doctor.service';
import {
  Receipt,
  CreateReceiptRequest,
  UpdateReceiptRequest,
  ReceiptTestEntry,
  LabReceiptStatus,
  ReportDeliveryStatus
} from '../../../../core/models/receipt.models';
import { DiagnosticTestListItem } from '../../../../core/models/diagnostic-test.models';

@Component({
  selector: 'app-receipt-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzCardModule,
    NzDatePickerModule,
    NzDividerModule,
    NzIconModule,
    NzTableModule,
    NzRadioModule,
    NzGridModule
  ],
  templateUrl: './receipt-form.component.html',
  styleUrls: ['./receipt-form.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReceiptFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly receiptService = inject(ReceiptService);
  private readonly diagnosticTestService = inject(DiagnosticTestService);
  private readonly doctorService = inject(DoctorService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly message: NzMessageService = inject(NzMessageService);

  receiptForm!: FormGroup;
  isEditMode = false;
  isViewMode = false;
  receiptId?: string;
  loading = false;
  submitting = false;

  availableTests = signal<DiagnosticTestListItem[]>([]);
  availableDoctors = signal<any[]>([]);
  loadingTests = signal<boolean>(false);
  loadingDoctors = signal<boolean>(false);

  testEntries = signal<ReceiptTestEntry[]>([]);

  readonly LabReceiptStatus = LabReceiptStatus;
  readonly ReportDeliveryStatus = ReportDeliveryStatus;

  readonly subTotal = computed(() => 
    this.testEntries().reduce((sum, entry) => sum + entry.rate, 0)
  );

  readonly total = computed(() => 
    this.subTotal() - (this.receiptForm.get('discount')?.value || 0)
  );

  readonly due = computed(() => this.total() - (this.receiptForm.get('balance')?.value || 0));

  ngOnInit(): void {
    const urlSegments = this.route.snapshot.url;
    const idParam = this.route.snapshot.paramMap.get('id');
    this.receiptId = idParam || undefined;
    
    if (urlSegments.length === 3 && urlSegments[2].path === 'edit') {
      this.isEditMode = true;
    } else if (urlSegments.length === 2 && idParam) {
      this.isViewMode = true;
    }

    this.initForm();
    this.loadTests();
    this.loadDoctors();

    if (this.receiptId) {
      this.loadReceipt(this.receiptId);
    }
  }

  initForm(): void {
    this.receiptForm = this.fb.group({
      selectedTest: [null],
      invoiceNumber: ['', [Validators.required]],
      fullName: ['', [Validators.required, Validators.maxLength(200)]],
      age: [null, [Validators.min(0), Validators.max(120)]],
      isAgeYear: [true],
      gender: ['Male', [Validators.required]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9+\-() ]+$/)]],
      address: ['', [Validators.maxLength(500)]],
      referredByDoctorId: [null],
      discount: [0, [Validators.min(0)]],
      balance: [0, [Validators.min(0)]],
      labReceiptStatus: [LabReceiptStatus.Pending, [Validators.required]],
      reportDeliveryStatus: [ReportDeliveryStatus.NotDelivered, [Validators.required]],
      reportDeliveredOn: [null]
    });
  }

  loadTests(): void {
    this.loadingTests.set(true);
    this.diagnosticTestService.getDiagnosticTests(undefined, undefined, true, 1, 1000).subscribe({
      next: (response) => {
        this.availableTests.set(response.data?.items || []);
        this.loadingTests.set(false);
      },
      error: () => {
        this.message.error('Failed to load diagnostic tests');
        this.loadingTests.set(false);
      }
    });
  }

  loadDoctors(): void {
    this.loadingDoctors.set(true);
    this.doctorService.getDoctors(undefined, 1, 1000).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.availableDoctors.set(response.data?.items || []);
        }
        this.loadingDoctors.set(false);
      },
      error: () => {
        this.message.error('Failed to load doctors');
        this.loadingDoctors.set(false);
      }
    });
  }

  loadReceipt(id: string): void {
    this.loading = true;
    this.receiptService.getReceipt(id).subscribe({
      next: (response) => {
        const receipt = response;
        this.receiptForm.patchValue({
          invoiceNumber: receipt.invoiceNumber,
          fullName: receipt.fullName,
          age: receipt.age,
          isAgeYear: receipt.isAgeYear,
          gender: receipt.gender,
          phoneNumber: receipt.phoneNumber,
          address: receipt.address,
          referredByDoctorId: receipt.referredByDoctorId,
          discount: receipt.discount,
          balance: receipt.balance,
          labReceiptStatus: receipt.labReceiptStatus,
          reportDeliveryStatus: receipt.reportDeliveryStatus,
          reportDeliveredOn: receipt.reportDeliveredOn ? new Date(receipt.reportDeliveredOn) : null
        });

        this.testEntries.set(receipt.testEntries || []);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading receipt:', error);
        this.message.error('Failed to load receipt');
        this.loading = false;
      }
    });
  }

  addTestEntry(): void {
    const testId = this.receiptForm.get('selectedTest')?.value;
    if (!testId) {
      this.message.warning('Please select a test');
      return;
    }

    const selectedTest = this.availableTests().find(t => t.id === testId);
    if (!selectedTest) {
      return;
    }

    const existingEntry = this.testEntries().find(e => e.diagnosticTestId === testId);
    if (existingEntry) {
      this.message.warning('Test already added');
      return;
    }

    debugger
    const newEntry: ReceiptTestEntry = {
      diagnosticTestId: selectedTest.id,
      diagnosticTestName: selectedTest.testName,
      rate: selectedTest.price,
      discount: 0,
      total: selectedTest.price
    };

    this.testEntries.set([...this.testEntries(), newEntry]);
    this.receiptForm.get('selectedTest')?.setValue(null);
  }

  removeTestEntry(index: number): void {
    const entries = [...this.testEntries()];
    entries.splice(index, 1);
    this.testEntries.set(entries);
  }

  updateTestEntry(index: number, field: 'discount', value: number): void {
    const entries = [...this.testEntries()];
    const entry = entries[index];
    
    if (field === 'discount') {
      entry.discount = value;
    }
    
    entry.total = Math.max(0, entry.rate - entry.discount);
    this.testEntries.set(entries);
  }

  onSubmit(): void {
    if (this.receiptForm.invalid) {
      Object.values(this.receiptForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    if (this.testEntries().length === 0) {
      this.message.warning('Please add at least one test');
      return;
    }

    this.submitting = true;
    const formValue = this.receiptForm.value;

    const testEntries = this.testEntries().map(entry => ({
      diagnosticTestId: entry.diagnosticTestId,
      rate: entry.rate,
      discount: entry.discount,
      total: entry.total
    }));

    const baseRequest = {
      invoiceNumber: formValue.invoiceNumber,
      fullName: formValue.fullName,
      age: formValue.age || undefined,
      isAgeYear: formValue.isAgeYear,
      gender: formValue.gender,
      phoneNumber: formValue.phoneNumber,
      address: formValue.address || undefined,
      referredByDoctorId: formValue.referredByDoctorId || undefined,
      subTotal: this.subTotal(),
      total: this.total(),
      discount: formValue.discount,
      due: this.due(),
      balance: formValue.balance,
      labReceiptStatus: formValue.labReceiptStatus,
      reportDeliveryStatus: formValue.reportDeliveryStatus,
      testEntries
    };

    if (this.isEditMode && this.receiptId) {
      const request: UpdateReceiptRequest = {
        ...baseRequest,
        id: this.receiptId,
        reportDeliveredOn: formValue.reportDeliveredOn?.toISOString() || undefined
      };

      this.receiptService.updateReceipt(this.receiptId, request).subscribe({
        next: (response) => {
          this.message.success('Receipt updated successfully');
          this.router.navigate(['/admin/receipts']);
          this.submitting = false;
        },
        error: (error) => {
          console.error('Error updating receipt:', error);
          this.message.error('Failed to update receipt');
          this.submitting = false;
        }
      });
    } else {
      const request: CreateReceiptRequest = baseRequest;

      this.receiptService.createReceipt(request).subscribe({
        next: (response) => {
          this.message.success('Receipt created successfully');
          this.router.navigate(['/admin/receipts']);
          this.submitting = false;
        },
        error: (error) => {
          console.error('Error creating receipt:', error);
          this.message.error('Failed to create receipt');
          this.submitting = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/admin/receipts']);
  }

  canEdit(): boolean {
    return !this.isViewMode;
  }

  getStatusText(status: LabReceiptStatus): string {
    switch (status) {
      case LabReceiptStatus.Pending:
        return 'Pending';
      case LabReceiptStatus.Paid:
        return 'Paid';
      case LabReceiptStatus.Due:
        return 'Due';
      case LabReceiptStatus.Void:
        return 'Void';
      default:
        return '';
    }
  }

  getDeliveryStatusText(status: ReportDeliveryStatus): string {
    return status === ReportDeliveryStatus.Delivered ? 'Delivered' : 'Not Delivered';
  }
}