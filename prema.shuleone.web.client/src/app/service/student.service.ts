import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Contact, Student, StudentPagination } from 'app/models/student.model';


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

  async getAllStudents(grade: number | undefined): Promise<Observable<Student[]>> {
    return this.http.get<Student[]>(`${this.apiUrl}/All?grade=${grade ?? 0}`); // Pass grade as 0 if it's undefined
  }

  async getRecentAdmissions(): Promise<Observable<Student[]>> {
    return this.http.get<Student[]>(`${this.apiUrl}/Admissions`);
  }

  async getStudent(id: number): Promise<Observable<Student>> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }

  async getStudentContact(id: number): Promise<Observable<Contact[]>> {
    return this.http.get<Contact[]>(`${this.apiUrl}/Contact/${id}`);
  }

  async getStudentsPaginated(page: number, perPage: number, admissionStatus: number, grade: number = 0): Promise<Observable<StudentPagination>> {
    // if(this.keycloakService.isUserInRole("super-admin") || this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("finance")){
      return this.http.get<StudentPagination>(`${this.apiUrl}?pageNumber=${page}&pageSize=${perPage}&admissionStatus=${admissionStatus}&grade=${grade}`);
    // }
  }
  
  async updateAdmissionStatus(admissionNumber: number): Promise<void> {
    try {
      await this.http
        .put<void>(`${this.apiUrl}/Admissions/UpdateStatus?admissionNumber=${admissionNumber}`, {})
        .toPromise();
      console.log('Update successful');
    } catch (error) {
      console.error('Update failed:', error);
    }
  }
  
  async downloadAdmissionLetter(admissionNumber: number): Promise<void> {
    await this.http.get(`${this.apiUrl}/Admission?admissionNumber=${admissionNumber}`, {
      responseType: 'blob',
      observe: 'response'
    }).subscribe(response => {
      const blob = new Blob([response.body!], { type: 'application/pdf' });
  
      // Extract filename from content-disposition header
      const contentDisposition = response.headers.get('content-disposition');
      let filename = admissionNumber + '_AdmissionLetter.pdf';
  
      if (contentDisposition) {
        const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
        if (matches != null && matches[1]) {
          filename = matches[1].replace(/['"]/g, '');
        }
      }
  
      // Create a temporary anchor element and trigger download
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = filename;
      link.click();
  
      // Cleanup
      window.URL.revokeObjectURL(link.href);
    }, error => {
      console.error('Download failed:', error);
    });
  }
  
}
