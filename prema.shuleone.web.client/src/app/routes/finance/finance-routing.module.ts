import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FinanceFeePaymentComponent } from './assign-fee-payment/assign-fee-payment.component';
import { FinanceMpesaStkPushComponent } from './mpesa-stk-push/mpesa-stk-push.component';
import { FinanceFeeReceivedComponent } from './fee-received/fee-received.component';
import { FinanceExpensesComponent } from './expenses/expenses.component';

const routes: Routes = [{ path: 'feePayment', component: FinanceFeePaymentComponent },
{ path: 'mpesaStkPush', component: FinanceMpesaStkPushComponent },
{ path: 'fee-received', component: FinanceFeeReceivedComponent },
{ path: 'expenses', component: FinanceExpensesComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinanceRoutingModule { }
