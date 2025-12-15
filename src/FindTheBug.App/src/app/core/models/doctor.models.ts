import { Response } from './auth.models';
import { PagedResult } from './common.models';

export interface DoctorSpeciality {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Doctor {
  id: string;
  name: string;
  degree?: string;
  office?: string;
  phoneNumber: string;
  specialities: DoctorSpeciality[];
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface DoctorListItem {
  id: string;
  name: string;
  degree?: string;
  phoneNumber: string;
  specialityNames: string[];
  isActive: boolean;
}

export interface CreateDoctorRequest {
  name: string;
  degree?: string;
  office?: string;
  phoneNumber: string;
  specialityIds: string[];
}

export interface UpdateDoctorRequest {
  name: string;
  degree?: string;
  office?: string;
  phoneNumber: string;
  specialityIds: string[];
  isActive: boolean;
}

// Response wrappers
export interface DoctorsResponse extends Response<Doctor[]> {}
export interface DoctorResponse extends Response<Doctor> {}
export interface DoctorSpecialitiesResponse extends Response<DoctorSpeciality[]> {}
export interface PagedDoctorsResponse extends Response<PagedResult<DoctorListItem>> {}
