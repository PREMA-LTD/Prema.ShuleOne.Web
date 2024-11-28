import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@shared/shared.module';

import { FinanceMpesaStkPushComponent } from './mpesa-stk-push.component';

describe('FinanceMpesaStkPushComponent', () => {
  let component: FinanceMpesaStkPushComponent;
  let fixture: ComponentFixture<FinanceMpesaStkPushComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [FinanceMpesaStkPushComponent],
      imports: [NoopAnimationsModule, SharedModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinanceMpesaStkPushComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
