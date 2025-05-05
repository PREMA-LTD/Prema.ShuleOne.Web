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

  export interface Expense {
    id?: number; // Assuming BaseTypeNoTracking includes an id
    description: string;
    amount: number; // decimal in C# maps to number in TypeScript
    fk_expense_subcategory_id: number;
    payment_reference: string;
    fk_from_account_id: number;
    fk_to_account_id: number;
    fk_transaction_id?: number; // nullable in C#, optional in TypeScript
    paid_by: string;
    date_paid?: Date;
    date_created: Date;
    reciept: string;
  }

  export interface ExpensePagination {
    total: number;
    expenses: Expense[];
  }

  export interface ExpenseDto {
    id: number;
    name: string;
    description: string;
    amount: number;
    category: string;
    payment_reference: string;
    fk_from_account_id: number;
    fk_to_account_id: number;
    fk_expense_subcategory_id?: number;
    fk_transaction_id?: number;
    paid_by: string;
    date_paid: Date | string;
    date_created: Date | string;
    reciept?: File;
  }
  

  
  export interface ExpenseCategories {
    id: number;
    name: string;
  }

  export interface ExpenseSubCategoryDto {
    id: number;
    name: string;
  }
  
  export interface ExpenseCategoryDto {
    id: number;
    name: string;
    expenseSubCategories: ExpenseSubCategoryDto[];
  }
  