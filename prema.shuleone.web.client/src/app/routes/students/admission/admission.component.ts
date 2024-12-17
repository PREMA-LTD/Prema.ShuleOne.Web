import { Component, inject, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MtxGridColumn } from '@ng-matero/extensions/grid';
import { AdmissionFormComponent } from './admission_form/admission-form.component';
import { MatDialog } from '@angular/material/dialog';
import { StudentService } from 'app/service/student.service';
import { finalize } from 'rxjs';
import { Student } from 'app/models/student.model';
import { KeycloakService } from 'keycloak-angular';

@Component({
  selector: 'app-students-admission',
  templateUrl: './admission.component.html',
  styleUrl: './admission.component.scss'
})
export class StudentsAdmissionComponent implements OnInit {

  constructor(public dialog: MatDialog) {}

  private readonly studentService = inject(StudentService);
  private readonly keycloakService = inject(KeycloakService);

  columns: MtxGridColumn[] = [
    { header: 'Admission No', field: 'id' },
    { header: 'Surname', field: 'surname' },
    { header: 'Other Names', field: 'other_names' },
    {
      header: 'Grade',
      field: 'current_grade',
      formatter: (data: any) => {
          switch (data.current_grade) {
              case 10:
                  return 'PlayGroup';
              case 11:
                  return 'PP1';
              case 12:
                  return 'PP2';
              default:
                  return data.current_grade;
          }
      }
  },
  
  
    // {
    //   header: 'Grade',
    //   field: 'grade',
    //   type: 'tag',
    //   tag: {
    //     1: { text: 'Paid', color: 'green-50' },
    //     2: { text: 'Pending', color: 'orange-50' },
    //     3: { text: 'Overdue', color: 'red-10' },
    //   },        
    // },
    {
      header: 'Action',
      field: 'action',
      type: 'button',
      buttons: [
        {
          text: 'Admission Letter',
          color: 'primary',
          icon: "download",
          iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
          click: (studentRecord: Student) => this.getAdmissionLetter(studentRecord)
        }
      ]
    }
  ];

  //#region Table Functions

  students: Student[] = [];
  total = 0;
  isLoading = true;

  query = {
    q: '',
    sort: 'stars',
    order: 'desc',
    page: 0,
    per_page: 10,
    grade: 0,
  };

  get params() {
    const p = Object.assign({}, this.query);
    p.page += 1;
    return p;
  }

  getNextPage(e: PageEvent) {
    this.query.page = e.pageIndex;
    this.query.per_page = e.pageSize;
    this.getStudents();
  }

  search() {
    this.query.page = 0;
    this.getStudents();
    console.log("query", JSON.stringify(this.query))
  }

  reset() {
    this.query.page = 0;
    this.query.per_page = 10;
    this.query.grade = 0;
    this.getStudents();
  }
  
  async getStudents() {
    this.isLoading = true;
    
    //for filtering
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);    

    (await this.studentService
      .getStudents())
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(res => {
        this.students = res.filter(student => new Date(student.date_of_admission) > oneMonthAgo);
        this.total = res.length;
        this.isLoading = false;
      });
  }

  admitNewStudent(){
    console.log("admitNewStudent")
    const dialogRef = this.dialog.open(AdmissionFormComponent, {
      width: '400px',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.success === true) {
      }
    });
  }

//#endregion

  async ngOnInit() {
    console.log("on init")
    await this.getStudents();
  }

  async getAdmissionLetter(studentRecord: Student) {

  }

}
