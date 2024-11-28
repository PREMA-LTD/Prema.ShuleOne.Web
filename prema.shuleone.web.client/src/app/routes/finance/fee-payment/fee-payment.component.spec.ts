import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@shared/shared.module';

import { FinanceFeePaymentComponent } from './fee-payment.component';

describe('FinanceFeePaymentComponent', () => {
  let component: FinanceFeePaymentComponent;
  let fixture: ComponentFixture<FinanceFeePaymentComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [FinanceFeePaymentComponent],
      imports: [NoopAnimationsModule, SharedModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinanceFeePaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
