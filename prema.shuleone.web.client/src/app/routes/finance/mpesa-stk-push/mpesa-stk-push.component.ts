import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-finance-mpesaStkPush',
  templateUrl: './mpesa-stk-push.component.html',
  styleUrl: './mpesa-stk-push.component.scss'
})
export class FinanceMpesaStkPushComponent implements OnInit {

  paymentData?: any;
  mpesaPaymentForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<FinanceMpesaStkPushComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.paymentData = data.paymentData;
    this.mpesaPaymentForm = this.fb.group({
      feeType: [this.paymentData.feeType, Validators.required],
      amount: [this.paymentData.amount, Validators.required],
      mpesaNumber: [
        this.paymentData.mpesaNumber,
        [
          Validators.required,
          Validators.pattern(/^254\d{9}$/)  // must start with '254' and have exactly 12 digits
        ]
      ],
    });
    
    this.mpesaPaymentForm.get('feeType')?.disable();    
    this.mpesaPaymentForm.get('amount')?.disable();
   }


  ngOnInit() {

  }

  submitForm() {}
}
