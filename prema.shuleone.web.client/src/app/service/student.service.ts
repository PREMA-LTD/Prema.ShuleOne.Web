import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Contact, Student } from 'app/models/student.model';


@Injectable({
  providedIn: 'root'
})

export class StudentService {
  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl + '/Student';

  constructor(private http: HttpClient) {}

  async admitStudent(studentDetails: Student): Promise<Student | undefined> {
    return this.http.post<Student>(`${this.apiUrl}/Admit`, studentDetails).toPromise();
  }

  async getAllStudents(): Promise<Observable<Student[]>> {
    return this.http.get<Student[]>(`${this.apiUrl}/All`);
  }

  async getRecentAdmissions(): Promise<Observable<Student[]>> {
    return this.http.get<Student[]>(`${this.apiUrl}/Admissions`);
  }

  async getStudent(id: number): Promise<Observable<Student>> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }

  async getStudentContact(id: number): Promise<Observable<Contact>> {
    return this.http.get<Contact>(`${this.apiUrl}/Contact/${id}`);
  }


}
