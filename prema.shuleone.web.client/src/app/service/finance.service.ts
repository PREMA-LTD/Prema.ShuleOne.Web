import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { Student } from 'app/models/student.model';


@Injectable({
  providedIn: 'root'
})

export class StudentService {
  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  async admitStudent(studentDetails: Student): Promise<Student | undefined> {
    return this.http.post<Student>(`${this.apiUrl}/Student/Admit`, studentDetails).toPromise();
  }

}
