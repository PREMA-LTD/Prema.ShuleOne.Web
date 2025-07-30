import { Component, inject, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MtxGridColumn } from '@ng-matero/extensions/grid';
import { MatDialog } from '@angular/material/dialog';
import { StudentService } from 'app/service/student.service';
import { finalize } from 'rxjs';
import { Student } from 'app/models/student.model';
import { KeycloakService } from 'keycloak-angular';
import { FinanceMpesaStkPushComponent } from 'app/routes/finance/mpesa-stk-push/mpesa-stk-push.component';
import {Expense, ExpenseDto} from 'app/models/finance.model';
import { FinanceService } from 'app/service/finance.service';
import { AccountingService } from 'app/service/accounting.service';
import { ExpenseDetailsComponent } from './expense_details/expense-details.component';
import { AddExpenseComponent } from './add_expense/add-expense.component';

@Component({
  selector: 'app-finance-expenses',
  templateUrl: './expenses.component.html',
  styleUrl: './expenses.component.scss'
})
export class FinanceExpensesComponent implements OnInit {

  constructor(public dialog: MatDialog) {}

  private readonly accountingService = inject(AccountingService);
  private readonly keycloakService = inject(KeycloakService);

  columns: MtxGridColumn[] = [
    { header: 'Id', field: 'id' },
    { header: 'Category', field: 'expense_category' },
    { header: 'Subcategory', field: 'expense_subcategory' },
    { header: 'Amount', field: 'amount' },
    { header: 'Payment Ref', field: 'payment_reference' },
    {
      header: 'Date Paid',
      field: 'date_paid',
      formatter: (row: any) => {
        const date = new Date(row.date_paid);
        return date.toLocaleDateString('en-GB');
      }
    },
    {
      header: 'Action',
      field: 'action',
      type: 'button',
      buttons: [
        {
          text: 'View Details',
          color: 'primary',
          icon: 'phone',
          // iif: (record: any) => record.fk_transaction_status_id !== 1 && (this.keycloakService.isUserInRole("admin") || this.keycloakService.isUserInRole("super-admin")),
          click: (record: any) => this.openExpenseDetails(record)
        }
      ]
    }
  ];

  //#region Table Functions

  expenses: ExpenseDto[] = [];
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
    this.getExpenses();
  }

  search() {
    this.query.page = 0;
    this.getExpenses();
    console.log("query", JSON.stringify(this.query))
  }

  reset() {
    this.query.page = 0;
    this.query.per_page = 10;
    this.query.grade = 0;
    this.getExpenses();
  }

  async getExpenses() {
    this.isLoading = true;

    (await this.accountingService
      .getAllExpensesPaginated(this.query.page, this.query.per_page))
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(res => {
        this.expenses = res.expenses;
        this.total = res.total;
        this.isLoading = false;
      });
  }

//#endregion


  openExpenseDetails(expenseDetails: Expense): void {
    const dialogRef = this.dialog.open(ExpenseDetailsComponent, {
      width: '400px',
      data: {
        expenseDetails
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.success === true) {
        // Refresh the table after a successful payment

      }
    });
  }


  openNewExpense() {
    const dialogRef = this.dialog.open(AddExpenseComponent, {
      width: '400px',
      data: {
        expenseDetails: null
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.success === true) {
        // Refresh the table after a successful payment
        this.getExpenses();

      }
    });
  }

  async ngOnInit() {
    console.log("on init")
    this.getExpenses();
  }


}
