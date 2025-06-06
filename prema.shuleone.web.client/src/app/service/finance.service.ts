import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Student } from 'app/models/student.model';
import { TransactionResult } from 'app/models/transansaction.model';
import { ExpensePagination } from 'app/models/finance.model';


@Injectable({
  providedIn: 'root'
})

export class FinanceService {
  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  async admitStudent(studentDetails: Student): Promise<Student | undefined> {
    return this.http.post<Student>(`${this.apiUrl}/Finance/Pay`, studentDetails).toPromise();
  }

  
  async initiateMpesaPayment(paymentDetails: any): Promise<any | undefined> {
    return this.http.post<any>(`${this.apiUrl}/Finance/InitiateMpesaPaymentPrompt`, paymentDetails).toPromise();
  }

  async checkPayment(transactionId: string): Promise<TransactionResult | undefined> {
    return this.http.get<TransactionResult>(`https://www.prema.co.ke/api/Transactions/TransactionStatus?transactionId=${transactionId}`).toPromise();
  }
  
}
