import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { FinanceRoutingModule } from './finance-routing.module';
import { FinanceFeePaymentComponent } from './fee-payment/fee-payment.component';
import { FinanceMpesaStkPushComponent } from './mpesa-stk-push/mpesa-stk-push.component';

const COMPONENTS: any[] = [FinanceFeePaymentComponent, FinanceMpesaStkPushComponent];
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
