export interface ReceiptListItem {
  id: string;
  invoiceNumber: string;
  fullName: string;
  phoneNumber: string;
  total: number;
  due: number;
  status: number;
  statusDisplay: string;
  createdAt: string;
}

export enum LabReceiptStatus {
  Pending = 0,
  Paid = 1,
  Due = 2,
  Void = 3
}

export enum ReportDeliveryStatus {
  NotDelivered = 0,
  Delivered = 1
}

export interface ReceiptTestEntry {
  id?: string;
  diagnosticTestId: string;
  diagnosticTestName?: string;
  rate: number;
  discount: number;
  total: number;
  status?: number;
  statusDisplay?: string;
}

export interface Receipt {
  id: string;
  invoiceNumber: string;
  fullName: string;
  age?: number;
  isAgeYear: boolean;
  gender: string;
  phoneNumber: string;
  address?: string;
  referredByDoctorId?: string;
  referredByDoctorName?: string;
  subTotal: number;
  total: number;
  discount: number;
  due: number;
  balance: number;
  reportDeliveredOn?: string;
  labReceiptStatus: LabReceiptStatus;
  reportDeliveryStatus: ReportDeliveryStatus;
  testEntries: ReceiptTestEntry[];
  createdAt: string;
  updatedAt?: string;
  createdBy?: string;
  updatedBy?: string;
}

export interface CreateReceiptRequest {
  invoiceNumber: string;
  fullName: string;
  age?: number;
  isAgeYear: boolean;
  gender: string;
  phoneNumber: string;
  address?: string;
  referredByDoctorId?: string;
  subTotal: number;
  total: number;
  discount: number;
  due: number;
  balance: number;
  labReceiptStatus: LabReceiptStatus;
  reportDeliveryStatus: ReportDeliveryStatus;
  testEntries: Omit<ReceiptTestEntry, 'id' | 'status' | 'statusDisplay'>[];
}

export interface UpdateReceiptRequest extends CreateReceiptRequest {
  id: string;
  reportDeliveredOn?: string;
}

export interface PagedReceiptsResult {
  items: ReceiptListItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}