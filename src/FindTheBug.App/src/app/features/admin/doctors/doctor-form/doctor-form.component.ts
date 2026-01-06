import { Component, OnInit, inject } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { DoctorService } from '../../../../core/services/doctor.service';
import { 
  Doctor, 
  CreateDoctorRequest, 
  UpdateDoctorRequest,
  DoctorSpeciality 
} from '../../../../core/models/doctor.models';

@Component({
  selector: 'app-doctor-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSwitchModule,
    NzSelectModule,
    NzCardModule
],
  templateUrl: './doctor-form.component.html',
  styleUrls: ['./doctor-form.component.css']
})
export class DoctorFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly doctorService = inject(DoctorService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly message = inject(NzMessageService);

  doctorForm!: FormGroup;
  isEditMode = false;
  doctorId?: string;
  loading = false;
  submitting = false;
  availableSpecialities: DoctorSpeciality[] = [];
  loadingSpecialities = false;

  ngOnInit(): void {
    this.doctorId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.doctorId;

    this.initForm();
    this.loadSpecialities();

    if (this.isEditMode && this.doctorId) {
      this.loadDoctor(this.doctorId);
    }
  }

  initForm(): void {
    this.doctorForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      degree: ['', Validators.maxLength(100)],
      office: ['', Validators.maxLength(200)],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[\d\s\-\(\)]+$/)]],
      specialityIds: [[], [Validators.required]],
      isActive: [true]
    });
  }

  loadSpecialities(): void {
    this.loadingSpecialities = true;
    this.doctorService.getDoctorSpecialities().subscribe({
      next: (response) => {
        if (response.isSuccess && response.data) {
          this.availableSpecialities = response.data;
        }
        this.loadingSpecialities = false;
      },
      error: () => {
        this.message.error('Failed to load doctor specialities');
        this.loadingSpecialities = false;
      }
    });
  }

  loadDoctor(id: string): void {
    this.loading = true;
    this.doctorService.getDoctor(id).subscribe({
      next: (response) => {
        if (response.isSuccess && response.data) {
          const doctor = response.data;
          this.doctorForm.patchValue({
            name: doctor.name,
            degree: doctor.degree || '',
            office: doctor.office || '',
            phoneNumber: doctor.phoneNumber,
            isActive: doctor.isActive,
            specialityIds: doctor.specialities.map(s => s.id)
          });
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading doctor:', error);
        this.message.error('Failed to load doctor');
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.doctorForm.invalid) {
      Object.values(this.doctorForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.submitting = true;
    const formValue = this.doctorForm.value;

    if (this.isEditMode && this.doctorId) {
      const request: UpdateDoctorRequest = {
        name: formValue.name,
        degree: formValue.degree || undefined,
        office: formValue.office || undefined,
        phoneNumber: formValue.phoneNumber,
        specialityIds: formValue.specialityIds,
        isActive: formValue.isActive
      };

      this.doctorService.updateDoctor(this.doctorId, request).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.message.success('Doctor updated successfully');
            this.router.navigate(['/admin/doctors']);
          } else {
            this.message.error(response.errorMessage || 'Failed to update doctor');
          }
          this.submitting = false;
        },
        error: (error) => {
          console.error('Error updating doctor:', error);
          this.message.error('Failed to update doctor');
          this.submitting = false;
        }
      });
    } else {
      const request: CreateDoctorRequest = {
        name: formValue.name,
        degree: formValue.degree || undefined,
        office: formValue.office || undefined,
        phoneNumber: formValue.phoneNumber,
        specialityIds: formValue.specialityIds
      };

      this.doctorService.createDoctor(request).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.message.success('Doctor created successfully');
            this.router.navigate(['/admin/doctors']);
          } else {
            this.message.error(response.errorMessage || 'Failed to create doctor');
          }
          this.submitting = false;
        },
        error: (error) => {
          console.error('Error creating doctor:', error);
          this.message.error('Failed to create doctor');
          this.submitting = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/admin/doctors']);
  }

  filterOption = (input: string, option: any): boolean => {
    return option.nzLabel.toLowerCase().includes(input.toLowerCase());
  };
}
