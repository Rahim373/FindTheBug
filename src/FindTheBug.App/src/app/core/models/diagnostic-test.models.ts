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
  testName: string;
  testCode: string;
  category: string;
  price: number;
  isActive: boolean;
}