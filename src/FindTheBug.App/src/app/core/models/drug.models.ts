import { Response } from './auth.models';
import { PagedResult } from './common.models';

export enum DrugType {
    Syrup = 'Syrup',
    Tablet = 'Tablet',
    Capsule = 'Capsule',
    Gel = 'Gel',
    Ointment = 'Ointment',
    Suspension = 'Suspension',
    Injection = 'Injection',
    Cream = 'Cream',
    Drops = 'Drops',
    Powder = 'Powder'
}

export interface Brand {
    id: string;
    name: string;
    isActive: boolean;
}

export interface GenericName {
    id: string;
    name: string;
    description?: string;
    isActive: boolean;
}

export interface Drug {
    id: string;
    name: string;
    strength: string;
    genericName: GenericName;
    brand: Brand;
    type: DrugType;
    unitPrice: number;
    photoPath?: string;
    isActive: boolean;
    createdAt: string;
    updatedAt?: string;
}

export interface DrugListItem {
    id: string;
    name: string;
    strength: string;
    genericName: string;
    brandName: string;
    type: DrugType;
    unitPrice: number;
    isActive: boolean;
}

export interface CreateDrugRequest {
    name: string;
    strength: string;
    genericNameId: string;
    brandId: string;
    type: DrugType;
    unitPrice: number;
    photoPath?: string;
}

export interface UpdateDrugRequest {
    name: string;
    strength: string;
    genericNameId: string;
    brandId: string;
    type: DrugType;
    unitPrice: number;
    photoPath?: string;
    isActive: boolean;
}

// Response wrappers
export interface DrugResponse extends Response<Drug> { }
export interface PagedDrugsResponse extends Response<PagedResult<DrugListItem>> { }
export interface BrandsResponse extends Response<Brand[]> { }
export interface GenericNamesResponse extends Response<GenericName[]> { }
