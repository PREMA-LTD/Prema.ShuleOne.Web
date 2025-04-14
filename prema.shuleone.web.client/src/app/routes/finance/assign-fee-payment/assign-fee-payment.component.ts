import { Component, Inject, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Student } from 'app/models/student.model';
import { AccountingService } from 'app/service/accounting.service';
import { StudentService } from 'app/service/student.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-assign-finance-feePayment',
  templateUrl: './assign-fee-payment.component.html',
  styleUrl: './assign-fee-payment.component.scss'
})
export class FinanceFeePaymentComponent implements OnInit {

  assignFeePaymentForm : FormGroup;
  students: Student[] = [];
  isLoading = false;

  private readonly studentService = inject(StudentService);
  private readonly accountingService = inject(AccountingService);
  
  grades = [
    { value: 10, name: 'Play Group' },
    { value: 11, name: 'PP1' },
    { value: 12, name: 'PP2' },
    { value: 1, name: 'Grade 1' },
    { value: 2, name: 'Grade 2' },
    { value: 3, name: 'Grade 3' },
    { value: 4, name: 'Grade 4' },
    { value: 5, name: 'Grade 5' },
    { value: 6, name: 'Grade 6' },
    { value: 6, name: 'Grade 7' },
    { value: 8, name: 'Grade 8' },
    { value: 9, name: 'Grade 9' }
  ];  

  revenueId: number = 0;

  constructor(
    private _snackBar: MatSnackBar, 
    private fb: FormBuilder, public dialog: MatDialog, 
    public dialogRef: MatDialogRef<FinanceFeePaymentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

      this.revenueId = data.revenueId;
      this.assignFeePaymentForm = this.fb.group({      
        grade: ['', Validators.required],
        student: ['', Validators.required]
      })
  }


  ngOnInit() {
    this.assignFeePaymentForm.get('student')?.disable();
  }

    
onGradeChange(grade: number) {
  this.getStudents(grade);
  this.assignFeePaymentForm.get('student')?.enable();
}

async getStudents(grade: number) {
  this.isLoading = true;
  
  (await this.studentService
    .getAllStudents(grade))
    .pipe(
      finalize(() => {
        this.isLoading = false;
      })
    )
    .subscribe(res => {
      this.students = res;
      this.isLoading = false;
    });
}

async assignPayment() {
  if (this.assignFeePaymentForm.invalid) {
    this.assignFeePaymentForm.markAllAsTouched();
    return;
  }

  this.isLoading = true;
  const studentId = this.assignFeePaymentForm.get('student')?.value;
  const revenueId = this.revenueId;

  try {
    const response = await this.accountingService.assignFeePayment(studentId, revenueId);
    if (response) {
      this._snackBar.open('Fee payment assigned successfully.', 'Ok', {
        horizontalPosition: 'right',
        verticalPosition: 'top',
        duration: 5000,
      });
      const paymentRecord: any = { response: response };
      this.dialogRef.close(paymentRecord);
    } else {
      this._snackBar.open('Error assigning fee payment, please try again.', 'Ok', {
        panelClass: ['error-snackbar'],
        horizontalPosition: 'right',
        verticalPosition: 'top',
        duration: 5000,
      });
    }
  } catch (error) {
    console.error('Error assigning fee payment:', error);
    this._snackBar.open('Error assigning fee payment, please try again.', 'Ok', {
      panelClass: ['error-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top',
      duration: 5000,
    });
  } finally {
    this.isLoading = false;
  }
}


}
