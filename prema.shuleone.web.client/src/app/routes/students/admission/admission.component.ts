import { Component, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MtxGridColumn } from '@ng-matero/extensions/grid';
import { AdmissionFormComponent } from './admission_form/admission-form.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-students-admission',
  templateUrl: './admission.component.html',
  styleUrl: './admission.component.scss'
})
export class StudentsAdmissionComponent implements OnInit {

  constructor(public dialog: MatDialog) {}


  columns: MtxGridColumn[] = [
    { header: 'Admission No', field: 'admission_no' },
    { header: 'Grade', field: 'grade' },
    { header: 'Name', field: 'name' }
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
    // {
    //   header: 'Action',
    //   field: 'action',
    //   type: 'button',
    //   buttons: [
    //     {
    //       text: 'Pay',
    //       color: 'primary',
    //       iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
    //       click: (record: any) => this.openPayModal(record)
    //     }
    //   ]
    // }
  ];


  students: any[] = [];
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

    // (await this.remoteSrv
    //   .getContributions(this.query.page, this.query.per_page, this.query.month, this.query.year, this.query. status, this.query.memberId))
    //   .pipe(
    //     finalize(() => {
    //       this.isLoading = false;
    //     })
    //   )
    //   .subscribe(res => {
    //     this.list = res.contributions;
    //     this.total = res.total;
    //     this.isLoading = false;
    //   });
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

  ngOnInit() {
  }

}
