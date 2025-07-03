import { Component, inject, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MtxGridColumn } from '@ng-matero/extensions/grid';
import { MatDialog } from '@angular/material/dialog';
import { StudentService } from 'app/service/student.service';
import { finalize } from 'rxjs';
import { Student } from 'app/models/student.model';
import { KeycloakService } from 'keycloak-angular';
import { FinanceMpesaStkPushComponent } from 'app/routes/finance/mpesa-stk-push/mpesa-stk-push.component';
import { AccountingService } from 'app/service/accounting.service';
import { RevenueStudentRecord, Revenue } from 'app/models/finance.model';
import { FinanceFeePaymentComponent } from '../assign-fee-payment/assign-fee-payment.component';

@Component({
  selector: 'app-finance-fee-received',
  templateUrl: './fee-received.component.html',
  styleUrl: './fee-received.component.scss'
})
export class FinanceFeeReceivedComponent implements OnInit {

  constructor(public dialog: MatDialog) {}


  private readonly accountingService = inject(AccountingService);
  private readonly keycloakService = inject(KeycloakService);

  async ngOnInit() {
    console.log("on init")
    await this.getRevenueRecords();
  }

//#region Table Functions
  columns: MtxGridColumn[] = [
      //revenue record
      { header: 'Id', field: 'revenue.id' },
      { header: 'Transaction Ref', field: 'revenue.payment_reference' },
      { header: 'Paid By', field: 'revenue.paid_by' },
      { header: 'Paid To', field: 'revenue.account_number' },
      { header: 'Paid On', field: 'revenue.payment_date' },
      { header: 'Amount', field: 'revenue.amount' },

      //student record
      {
        header: 'Admission',
        field: 'student.id',
        formatter: (data: any | undefined) => {
          if (!data.student) {
            return "Invalid account number.";
          } else {
            return data.student.id;
          }
        }
      },
      { header: 'Surname', field: 'student.surname' },
      { header: 'Other Names', field: 'student.other_names' },
      {
        header: 'Grade',
        field: 'student.current_grade',
        formatter: (data: any | undefined) => {
          if (!data.student) {
            return "";
          }
            switch (data.student.current_grade) {
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
                    return data.student.current_grade;
            }
        }
    },
    {
      header: 'Action',
      field: 'action',
      type: 'button',
      buttons: [
        {
          text: 'Assign Payment.',
          color: 'primary',
          icon: 'money',
          //iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
          iif: (record: RevenueStudentRecord) => record.student === null,
          click: (record: RevenueStudentRecord) => this.openFeeAssignDialog(record)
        },
        {
          text: 'Receipt',
          color: 'primary',
          icon: 'download',
          //iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
          iif: (record: RevenueStudentRecord) => record.student != null,
          click: (record: RevenueStudentRecord) => this.downloadReceipt(record)
        }
      ]
    }
  ];

  revenueRecords: RevenueStudentRecord[] = [];
  total = 0;
  isLoading = true;

  query = {
    q: '',
    sort: 'stars',
    order: 'desc',
    page: 0,
    per_page: 10,
    grade: 0,
    account: null,
    transactionRef: null,
    dateFrom: null,
    dateTo: null
  };

  get params() {
    const p = Object.assign({}, this.query);
    p.page += 1;
    return p;
  }

  getNextPage(e: PageEvent) {
    this.query.page = e.pageIndex;
    this.query.per_page = e.pageSize;
    this.getRevenueRecords();
  }

  search() {
    this.query.page = 0;
    this.getRevenueRecords();
    console.log("query", JSON.stringify(this.query))
  }

  reset() {
    this.query.page = 0;
    this.query.per_page = 10;
    this.query.grade = 0;
    this.getRevenueRecords();
  }

  async getRevenueRecords() {
    this.isLoading = true;

    (await this.accountingService
      .getRevenuePaginated(this.query.page, this.query.per_page, this.query.account, this.query.transactionRef, this.query.dateFrom, this.query.dateTo))
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(res => {
        this.revenueRecords = res.revenueStudentRecords;
        this.total = res.total;
        this.isLoading = false;
      });
  }



//#endregion


openFeeAssignDialog(revenueRecord: RevenueStudentRecord): void {
    const dialogRef = this.dialog.open(FinanceFeePaymentComponent, {
      width: '400px',
      data: {
        revenueId: revenueRecord.revenue.id
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Refresh the table after a successful payment
        this.getRevenueRecords();
      }
    });
  }

  downloadReceipt(revenueStudentRecord: RevenueStudentRecord): void {
    this.accountingService.getReceipt(revenueStudentRecord.revenue.id).subscribe({
      next: (blob: Blob) => {
        const fileName = `receipt-${revenueStudentRecord.revenue.id}-${revenueStudentRecord.student?.id} ${revenueStudentRecord.student?.other_names} ${revenueStudentRecord.student?.surname}.pdf`;
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(link.href);
      },
      error: (err) => {
        console.error('Download failed', err);
      }
    });
  }


}
