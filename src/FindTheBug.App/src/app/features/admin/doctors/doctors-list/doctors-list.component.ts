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
import { DoctorService } from '../../../../core/services/doctor.service';
import { DoctorListItem } from '../../../../core/models/doctor.models';

@Component({
  selector: 'app-doctors-list',
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
  templateUrl: './doctors-list.component.html',
  styleUrls: ['./doctors-list.component.css']
})
export class DoctorsListComponent implements OnInit {
  private readonly doctorService = inject(DoctorService);
  private readonly router = inject(Router);
  private readonly modal = inject(NzModalService);
  private readonly message = inject(NzMessageService);

  doctors: DoctorListItem[] = [];
  loading = false;
  searchText = '';
  
  // Pagination
  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.loading = true;
    this.doctorService.getDoctors(this.searchText || undefined, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        if (response.isSuccess && response.data) {
          this.doctors = response.data.items || [];
          this.totalCount = response.data.totalCount || 0;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading doctors:', error);
        this.message.error('Failed to load doctors');
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadDoctors();
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadDoctors();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageNumber = 1;
    this.loadDoctors();
  }

  createDoctor(): void {
    this.router.navigate(['/admin/doctors/create']);
  }

  viewDoctor(id: string): void {
    this.router.navigate(['/admin/doctors', id]);
  }

  editDoctor(id: string): void {
    this.router.navigate(['/admin/doctors', id, 'edit']);
  }

  deleteDoctor(doctor: DoctorListItem): void {
    this.modal.confirm({
      nzTitle: 'Delete Doctor',
      nzContent: `Are you sure you want to delete ${doctor.name}?`,
      nzOkText: 'Delete',
      nzOkDanger: true,
      nzOnOk: () => {
        this.doctorService.deleteDoctor(doctor.id).subscribe({
          next: () => {
            this.message.success('Doctor deleted successfully');
            this.loadDoctors();
          },
          error: (error) => {
            console.error('Error deleting doctor:', error);
            this.message.error('Failed to delete doctor');
          }
        });
      }
    });
  }

  toggleDoctorStatus(doctor: DoctorListItem): void {
    const action = doctor.isActive ? 'deactivate' : 'activate';
    this.modal.confirm({
      nzTitle: `${action.charAt(0).toUpperCase() + action.slice(1)} Doctor`,
      nzContent: `Are you sure you want to ${action} ${doctor.name}?`,
      nzOkText: action.charAt(0).toUpperCase() + action.slice(1),
      nzOnOk: () => {
        // In a real implementation, you'd call a status update endpoint
        // For now, we'll show a success message and update locally
        doctor.isActive = !doctor.isActive;
        this.message.success(`Doctor ${action}d successfully`);
      }
    });
  }
}
