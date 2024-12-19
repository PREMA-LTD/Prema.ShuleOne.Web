import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FinanceService } from 'app/service/finance.service';

@Component({
  selector: 'app-finance-mpesaStkPush',
  templateUrl: './mpesa-stk-push.component.html',
  styleUrl: './mpesa-stk-push.component.scss'
})
export class FinanceMpesaStkPushComponent implements OnInit {

  private readonly financeService = inject(FinanceService);

  paymentData?: any;
  mpesaPaymentForm!: FormGroup;
  recordPaymentForm!: FormGroup;

  constructor(
    private _snackBar: MatSnackBar,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<FinanceMpesaStkPushComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.paymentData = data.paymentData;
    
    console.log("FinanceMpesaStkPushComponent paymentdetails"+JSON.stringify(this.paymentData));
    this.mpesaPaymentForm = this.fb.group({
      feeType: [this.paymentData.feeType, Validators.required],
      amount: [this.paymentData.amount, Validators.required],
      mpesaNumber: [
        this.paymentData.mpesaNumber,
        [
          Validators.required,
          Validators.pattern(/^0\d{9}$/) // must start with '254' and have exactly 12 digits
        ]
      ],
    });


    this.recordPaymentForm = this.fb.group({
      accountNumber: ['', Validators.required]
    });

    this.mpesaPaymentForm.get('feeType')?.disable();    
    this.mpesaPaymentForm.get('amount')?.disable();
   }


  ngOnInit() {

  }

  async initiateMpesaPrompt() {
    try {
      
        const paymentDetails = this.mpesaPaymentForm.value;

        const phoneNumber = paymentDetails.mpesaNumber;
        const formattedNumber = phoneNumber.replace(/^0/, "254");

        paymentDetails.mpesaNumber = formattedNumber;
        paymentDetails.feeType = this.paymentData.feeType;
        paymentDetails.amount = this.paymentData.amount;

        console.log("submitForm mpesa-stk-push "+JSON.stringify(paymentDetails))

        const response = await this.financeService.initiateMpesaPayment(paymentDetails)
      
        if(response){
        this._snackBar.open('Mpesa payment prompt sent successfully.', 'Ok', {
          horizontalPosition: 'right',
          verticalPosition: 'top',
          duration: 5 * 1000,
          });
  
          
        } else {
          this._snackBar.open('Error sending mpesa prompt, please try again.', 'Ok', {
            panelClass: ['error-snackbar'],  // Add a custom CSS class
            horizontalPosition: 'right',
            verticalPosition: 'top',
            duration: 5 * 1000,
            });
        }
  
      
      } catch(error) {
        console.error('Error sending message:', error);
  
        
        this._snackBar.open('Error sending mpesa prompt, please try again.', 'Ok', {
          panelClass: ['error-snackbar'],  // Add a custom CSS class
          horizontalPosition: 'right',
          verticalPosition: 'top',
          duration: 5 * 1000,
          });
      };
  }

  recordPayment() {}
}
