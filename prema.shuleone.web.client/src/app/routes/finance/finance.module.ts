import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { FinanceRoutingModule } from './finance-routing.module';
import { FinanceFeePaymentComponent } from './assign-fee-payment/assign-fee-payment.component';
import { FinanceMpesaStkPushComponent } from './mpesa-stk-push/mpesa-stk-push.component';
import { FinanceFeeReceivedComponent } from './fee-received/fee-received.component';
import { FinanceExpensesComponent } from './expenses/expenses.component';
import { ExpenseDetailsComponent } from './expenses/expense_details/expense-details.component';
import { AddExpenseComponent } from './expenses/add_expense/add-expense.component';

const COMPONENTS: any[] = [
  FinanceFeePaymentComponent, 
  FinanceMpesaStkPushComponent, 
  FinanceFeeReceivedComponent, 
  FinanceExpensesComponent, 
  ExpenseDetailsComponent,
  AddExpenseComponent];
const COMPONENTS_DYNAMIC: any[] = [];

@NgModule({
  imports: [
    SharedModule,
    FinanceRoutingModule
  ],
  declarations: [
    ...COMPONENTS,
    ...COMPONENTS_DYNAMIC
  ]
})
export class FinanceModule { }
