import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Student } from 'app/models/student.model';
import { TransactionResult } from 'app/models/transansaction.model';
import { RevenueStudentRecord, RevenueStudentRecordsPagination } from 'app/models/finance.model';


@Injectable({
  providedIn: 'root'
})

export class AccountingService {
  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl + '/Accounting';

  constructor(private http: HttpClient) {}

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
  
  
}
