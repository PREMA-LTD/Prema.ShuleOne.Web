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
  selector: 'app-finance-expenses',
  templateUrl: './expenses.component.html',
  styleUrl: './expenses.component.css'
})
export class FinanceExpensesComponent implements OnInit {

  constructor(public dialog: MatDialog) {}

  private readonly studentService = inject(StudentService);
  private readonly keycloakService = inject(KeycloakService);

  columns: MtxGridColumn[] = [
    { header: 'Admission No', field: 'id' },
    { header: 'Surname', field: 'surname' },
    { header: 'Other Names', field: 'other_names' },
    {
      header: 'Action',
      field: 'action',
      type: 'button',
      buttons: [    
        {
          text: 'View Contact',
          color: 'primary',
          icon: 'phone',
          // iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
          click: (record: any) => this.openContactInfo(record)
        }  
      ]
    }
  ];

  //#region Table Functions

  students: Expense[] = [];
  total = 0;
  isLoading = true;

  query = {
    q: '',
    sort: 'stars',
    order: 'desc',
    page: 0,
    per_page: 10,
    grade: 0,
    admissionStatus: 0
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
    
    (await this.studentService
      .getStudentsPaginated(this.query.page, this.query.per_page, this.query.admissionStatus, this.query.grade))
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(res => {
        this.students = res.students;
        this.total = res.total;
        this.isLoading = false;
      });
  }



//#endregion

  async ngOnInit() {
    console.log("on init")
    await this.getStudents();
  }

  openContactInfo(studentRecord: any): void {
    const dialogRef = this.dialog.open(ContactInfoComponent, {
      width: '400px',
      data: { 
        studentRecord
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.success === true) {
        // Refresh the table after a successful payment

      }
    });
  }  

}
