import { Student } from "./student.model";

interface Revenue {
    id: number;
    amount: number;
    paid_by: string;
    payment_reference: string;
    account_number: string;
    fk_intended_account_number: number | null;
    status: number;
    payment_date: string; // ISO date string
    payment_method: number;
    date_created: string; // ISO date string
    recorded_by: string;
  }
  
  export interface RevenueStudentRecord {
    revenue: Revenue;
    student: Student | null;
  }  
    
  export interface RevenueStudentRecordsPagination {
    total: number;
    revenueStudentRecords: RevenueStudentRecord[];
  }