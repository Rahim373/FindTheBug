import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ReceiptService } from '../../../../core/services/receipt.service';
import { DoctorService } from '../../../../core/services/doctor.service';
import { DiagnosticTestService } from '../../../../core/services/diagnostic-test.service';
import { 
  Receipt, 
  CreateReceiptRequest,
  UpdateReceiptRequest,
  LabReceiptStatus,
  ReportDeliveryStatus 
} from '../../../../core/models/receipt.models';
import { DiagnosticTestListItem } from '../../../../core/models/diagnostic-test.models';
import { NzTableModule } from 'ng-zorro-antd/table';

@Component({
  selector: 'app-receipt-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzSwitchModule,
    NzRadioModule,
    NzInputNumberModule,
    NzDatePickerModule,
    NzCardModule,
    NzDividerModule,
    NzIconModule,
    NzTableModule,
    NzInputModule,
  ],
  templateUrl: './receipt-form.component.html',
  styleUrls: ['./receipt-form.component.css']
})
export class ReceiptFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly receiptService = inject(ReceiptService);
  private readonly doctorService = inject(DoctorService);
  private readonly diagnosticTestService = inject(DiagnosticTestService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly message = inject(NzMessageService);

  receiptForm!: FormGroup;
  isEditMode = false;
  receiptId?: string;
  loading = false;
  submitting = false;
  doctors: any[] = [];
  loadingDoctors = false;
  diagnosticTests: DiagnosticTestListItem[] = [];
  loadingDiagnosticTests = false;
  diagnosticTestMap: Map<string, DiagnosticTestListItem> = new Map();

  readonly LabReceiptStatus = LabReceiptStatus;
  readonly ReportDeliveryStatus = ReportDeliveryStatus;

  ngOnInit(): void {
    this.receiptId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.receiptId;

    this.initForm();
    this.loadDoctors();
    this.loadDiagnosticTests();

    if (this.isEditMode && this.receiptId) {
      this.loadReceipt(this.receiptId);
    }
  }

  initForm(): void {
    this.receiptForm = this.fb.group({
      invoiceNumber: ['', [Validators.required, Validators.maxLength(50)]],
      fullName: ['', [Validators.required, Validators.maxLength(200)]],
      age: [null],
      isAgeYear: [true],
      gender: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[\d\s\-\(\)]+$/)]],
      address: ['', Validators.maxLength(500)],
      referredByDoctorId: [null],
      subTotal: [0, [Validators.required, Validators.min(0)]],
      total: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.required, Validators.min(0)]],
      due: [0, [Validators.required, Validators.min(0)]],
      balance: [0, [Validators.required, Validators.min(0)]],
      labReceiptStatus: [LabReceiptStatus.Pending, Validators.required],
      reportDeliveryStatus: [ReportDeliveryStatus.NotDelivered, Validators.required],
      reportDeliveredOn: [null],
      testEntries: this.fb.array([])
    });
  }

  loadDoctors(): void {
    this.loadingDoctors = true;
    this.doctorService.getDoctors(undefined, 1, 1000).subscribe({
      next: (response) => {
        if (response && response.isSuccess && response.data) {
          this.doctors = response.data.items || [];
        }
        this.loadingDoctors = false;
      },
      error: () => {
        this.message.error('Failed to load doctors');
        this.loadingDoctors = false;
      }
    });
  }

  loadDiagnosticTests(): void {
    this.loadingDiagnosticTests = true;
    this.diagnosticTestService.getDiagnosticTests(undefined, undefined, true, 1, 1000).subscribe({
      next: (response) => {
        if (response) {
          this.diagnosticTests = response.items || [];
          // Build map for quick lookup
          this.diagnosticTestMap = new Map(
            this.diagnosticTests.map(test => [test.id, test])
          );
        }
        this.loadingDiagnosticTests = false;
      },
      error: () => {
        this.message.error('Failed to load diagnostic tests');
        this.loadingDiagnosticTests = false;
      }
    });
  }

  loadReceipt(id: string): void {
    this.loading = true;
    this.receiptService.getReceipt(id).subscribe({
      next: (receipt) => {
        this.receiptForm.patchValue({
          invoiceNumber: receipt.invoiceNumber,
          fullName: receipt.fullName,
          age: receipt.age,
          isAgeYear: receipt.isAgeYear,
          gender: receipt.gender,
          phoneNumber: receipt.phoneNumber,
          address: receipt.address || '',
          referredByDoctorId: receipt.referredByDoctorId,
          subTotal: receipt.subTotal,
          total: receipt.total,
          discount: receipt.discount,
          due: receipt.due,
          balance: receipt.balance,
          labReceiptStatus: receipt.labReceiptStatus,
          reportDeliveryStatus: receipt.reportDeliveryStatus,
          reportDeliveredOn: receipt.reportDeliveredOn ? new Date(receipt.reportDeliveredOn) : null
        });

        // Load test entries
        const testEntriesArray = this.receiptForm.get('testEntries') as FormArray;
        testEntriesArray.clear();
        
        receipt.testEntries.forEach(test => {
          this.addTestEntry({
            diagnosticTestId: test.diagnosticTestId,
            diagnosticTestName: test.diagnosticTestName,
            rate: test.rate || 0,
            discount: test.discount || 0,
            total: test.total
          });
        });

        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading receipt:', error);
        this.message.error('Failed to load receipt');
        this.loading = false;
      }
    });
  }

  get testEntries(): FormArray {
    return this.receiptForm.get('testEntries') as FormArray;
  }

  createTestEntry(testData?: any): FormGroup {
    const testId = testData?.diagnosticTestId || '';
    const diagnosticTest = this.diagnosticTestMap.get(testId);
    const rate = diagnosticTest?.rate || testData?.rate || 0;
    
    const group = this.fb.group({
      diagnosticTestId: [testId, Validators.required],
      diagnosticTestName: [testData?.diagnosticTestName || ''],
      rate: [{ value: rate, disabled: true }, Validators.required],
      discount: [testData?.discount || 0, [Validators.required, Validators.min(0)]],
      total: [testData?.total || 0, [Validators.required, Validators.min(0)]]
    });

    // Auto-calculate total when discount changes
    group.get('discount')?.valueChanges.subscribe(() => {
      this.calculateTestEntryTotal(group);
    });

    return group;
  }

  calculateTestEntryTotal(group: FormGroup): void {
    const rate = group.get('rate')?.value || 0;
    const discount = group.get('discount')?.value || 0;
    const total = Math.max(0, rate - discount);
    group.get('total')?.setValue(total, { emitEvent: false });
  }

  onDiagnosticTestChange(index: number, testId: string): void {
    const testEntriesArray = this.receiptForm.get('testEntries') as FormArray;
    const group = testEntriesArray.at(index) as FormGroup;
    
    const diagnosticTest = this.diagnosticTestMap.get(testId);
    if (diagnosticTest) {
      group.get('diagnosticTestName')?.setValue(diagnosticTest.name);
      group.get('rate')?.setValue(diagnosticTest.rate);
      group.get('discount')?.setValue(0);
      this.calculateTestEntryTotal(group);
    }
  }

  addTestEntry(testData?: any): void {
    this.testEntries.push(this.createTestEntry(testData));
  }

  removeTestEntry(index: number): void {
    this.testEntries.removeAt(index);
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

    this.submitting = true;
    const formValue = this.receiptForm.value;

    // Prepare test entries
    const testEntries = formValue.testEntries.map((te: any) => ({
      diagnosticTestId: te.diagnosticTestId,
      rate: te.rate,
      discount: te.discount,
      total: te.total
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
      subTotal: formValue.subTotal,
      total: formValue.total,
      discount: formValue.discount,
      due: formValue.due,
      balance: formValue.balance,
      labReceiptStatus: formValue.labReceiptStatus,
      reportDeliveryStatus: formValue.reportDeliveryStatus,
      testEntries: testEntries
    };

    if (this.isEditMode && this.receiptId) {
      const request: UpdateReceiptRequest = {
        ...baseRequest,
        id: this.receiptId,
        reportDeliveredOn: formValue.reportDeliveredOn ? formValue.reportDeliveredOn.toISOString() : undefined
      };

      this.receiptService.updateReceipt(this.receiptId, request).subscribe({
        next: (response) => {
          this.message.success('Receipt updated successfully');
          this.router.navigate(['/admin/receipts']);
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

  filterOption = (input: string, option: any): boolean => {
    return option.nzLabel.toLowerCase().includes(input.toLowerCase());
  };
}