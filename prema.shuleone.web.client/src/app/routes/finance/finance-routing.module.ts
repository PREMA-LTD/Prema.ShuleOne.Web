import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FinanceFeePaymentComponent } from './fee-payment/fee-payment.component';
import { FinanceMpesaStkPushComponent } from './mpesa-stk-push/mpesa-stk-push.component';

const routes: Routes = [{ path: 'feePayment', component: FinanceFeePaymentComponent },
{ path: 'mpesaStkPush', component: FinanceMpesaStkPushComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinanceRoutingModule { }
