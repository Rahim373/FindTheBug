export interface Expense {
  id: string;
  date: string;
  note: string;
  amount: number;
  paymentMethod: string;
  referenceNo?: string;
  attachment?: string;
  department: string;
  createdAt: string;
  updatedAt?: string;
}

export interface ExpenseListItem {
  id: string;
  date: string;
  note: string;
  amount: number;
  paymentMethod: string;
  referenceNo?: string;
  department: string;
}

export interface PaginatedExpenseList {
  items: ExpenseListItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface CreateExpenseRequest {
  date: string;
  note: string;
  amount: number;
  paymentMethod: string;
  referenceNo?: string;
  attachment?: string;
  department: string;
}

export interface UpdateExpenseRequest {
  id: string;
  date: string;
  note: string;
  amount: number;
  paymentMethod: string;
  referenceNo?: string;
  attachment?: string;
  department: string;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  errors?: any[];
  message?: string;
}

export enum PaymentMethod {
  Cash = 'Cash',
  Cheque = 'Cheque',
  bKash = 'bKash',
  Nagad = 'Nagad'
}

export enum Department {
  Lab = 'Lab',
  Dispensary = 'Dispensary'
}