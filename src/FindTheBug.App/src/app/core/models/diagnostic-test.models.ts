export interface DiagnosticTest {
  id: string;
  name: string;
  testCode: string;
  category: string;
  rate: number;
  isActive: boolean;
}

export interface DiagnosticTestListItem {
  id: string;
  name: string;
  testCode: string;
  category: string;
  rate: number;
  isActive: boolean;
}

export interface PagedDiagnosticTestsResult {
  items: DiagnosticTestListItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}