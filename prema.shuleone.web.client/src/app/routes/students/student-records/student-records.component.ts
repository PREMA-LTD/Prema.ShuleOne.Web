import { Component, inject, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MtxGridColumn } from '@ng-matero/extensions/grid';
import { MatDialog } from '@angular/material/dialog';
import { StudentService } from 'app/service/student.service';
import { finalize } from 'rxjs';
import { Student } from 'app/models/student.model';
import { KeycloakService } from 'keycloak-angular';
import { FinanceMpesaStkPushComponent } from 'app/routes/finance/mpesa-stk-push/mpesa-stk-push.component';

@Component({
  selector: 'app-students-StudentRecords',
  templateUrl: './student-records.component.html',
  styleUrl: './student-records.component.scss'
})
export class StudentsStudentRecordsComponent implements OnInit {

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
              case 1:
                  return 'Grade 1';
              case 2:
                  return 'Grade 2';
              case 3:
                  return 'Grade 3';
              case 4:
                  return 'Grade 4';
              case 5:
                  return 'Grade 5';
              case 6:
                  return 'Grade 6';
              case 7:
                  return 'Grade 7';
              case 8:
                  return 'Grade 8';
              case 9:
                  return 'Grade 9';
              default:
                  return data.current_grade;
          }
      }
  },
  
    {
      header: 'Payment Status',
      field: 'admission_status',
      type: 'tag',
      tag: {
        1: { text: 'Unpaid', color: 'orange-50' },
        2: { text: 'Paid', color: 'green-50' }
      },        
    },
    {
      header: 'Action',
      field: 'action',
      type: 'button',
      buttons: [      
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
      .getAllStudents())
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



//#endregion

  async ngOnInit() {
    console.log("on init")
    await this.getStudents();
  }

}
