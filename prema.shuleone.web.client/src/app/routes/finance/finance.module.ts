import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { FinanceRoutingModule } from './finance-routing.module';
import { FinanceFeePaymentComponent } from './assign-fee-payment/assign-fee-payment.component';
import { FinanceMpesaStkPushComponent } from './mpesa-stk-push/mpesa-stk-push.component';
import { FinanceFeeReceivedComponent } from './fee-received/fee-received.component';
import { FinanceExpensesComponent } from './expenses/expenses.component';

const COMPONENTS: any[] = [FinanceFeePaymentComponent, FinanceMpesaStkPushComponent, FinanceFeeReceivedComponent, FinanceExpensesComponent];
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
