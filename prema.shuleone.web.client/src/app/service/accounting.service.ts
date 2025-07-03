import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Student } from 'app/models/student.model';
import { TransactionResult } from 'app/models/transansaction.model';
import { ExpenseCategoryDto, ExpenseDto, ExpensePagination, RevenueStudentRecord, RevenueStudentRecordsPagination } from 'app/models/finance.model';


@Injectable({
  providedIn: 'root'
})

export class AccountingService {

  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl + '/Accounting';

  constructor(private http: HttpClient) {}

  private getAntiForgeryToken(): string {
    const cookieName = '.AspNetCore.Antiforgery.';
    const cookies = document.cookie.split(';');
    for (const cookie of cookies) {
      const trimmedCookie = cookie.trim();
      if (trimmedCookie.startsWith(cookieName)) {
        return trimmedCookie.substring(cookieName.length + 1);
      }
    }
    return '';
  }

  async getRevenuePaginated(page: number, perPage: number, account: number | undefined | null = null, transactionRef: string | undefined | null, dateFrom: Date | undefined | null = null, dateTo: | undefined | null = null): Promise<Observable<RevenueStudentRecordsPagination>> {
    // if(this.keycloakService.isUserInRole("super-admin") || this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("finance")){
    let parameters = ``;

    if(account !== null){
      parameters += `&account=${account}`;
    }

    if(transactionRef !== null){
      parameters += `&transactionRef=${transactionRef}`;
    }

    if(dateFrom !== null && account !== null){
      parameters += `&dateFrom=${dateFrom}&dateTo=${dateTo}`;
    }

    if (parameters != ``){
      return this.http.get<RevenueStudentRecordsPagination>(`${this.apiUrl}/Revenue/All?pageNumber=${page}&pageSize=${perPage}${parameters}`);
    } else {
      return this.http.get<RevenueStudentRecordsPagination>(`${this.apiUrl}/Revenue/All?pageNumber=${page}&pageSize=${perPage}`);
    }
  }

  async assignFeePayment(studentId: number, revenueId: number): Promise<RevenueStudentRecord> {
    const url = `${this.apiUrl}/Revenue/ManualUpdate`;
    const params = new HttpParams()
      .set('studentId', studentId.toString())
      .set('revenueId', revenueId.toString());

    const response = await this.http.post<RevenueStudentRecord>(url, null, { params }).toPromise();

    if (response === undefined) {
      throw new Error("API returned an undefined response");
    }

    return response;
  }

  async checkPaymentStatus(transactionRef: boolean): Promise<boolean> {
    try {
      const result = await this.http
        .get<{ status: boolean }>(`${this.apiUrl}/CheckPayment?transactionReference=${transactionRef}`)
        .toPromise();

        return result?.status ?? false; // fallback to false if result is null or undefined
    } catch (error) {
      console.error('Failed to check payment status:', error);
      return false;
    }
  }

  async getAllExpensesPaginated(page: number, perPage: number): Promise<Observable<ExpensePagination>> {
    // if(this.keycloakService.isUserInRole("super-admin") || this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("finance")){
      return this.http.get<ExpensePagination>(`${this.apiUrl}/Expense/All?pageNumber=${page}&pageSize=${perPage}`);
    // }
  }

  async createExpense(formData: FormData) {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/Expense`, formData)
      .pipe(
        catchError(error => {
          console.error('Error creating expense:', error);
          return throwError(() => new Error('Failed to create expense'));
        })
      ).toPromise();
  }

  async getExpenseCategories(): Promise<Observable<ExpenseCategoryDto[]>> {
      return this.http.get<ExpenseCategoryDto[]>(`${this.apiUrl}/Expense/Categories`);
  }

  getReceipt(revenueId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/Receipt/${revenueId}`, { responseType: 'blob' });
  }

  // createExpense(formData: FormData): Observable<ExpenseDto> {
  //   // Get the anti-forgery token from the cookie (if available)
  //   const token = this.getAntiForgeryToken();

  //   // Create headers with the anti-forgery token
  //   const headers = new HttpHeaders({
  //     'X-XSRF-TOKEN': token
  //   });

  //   // Send the request with headers
  //   return this.http.post<ExpenseDto>(`${this.apiUrl}/Expense`, formData, { headers })
  //     .pipe(
  //       catchError(error => {
  //         console.error('Error creating expense:', error);
  //         return throwError(() => new Error('Failed to create expense'));
  //       })
  //     );
  // }
}
