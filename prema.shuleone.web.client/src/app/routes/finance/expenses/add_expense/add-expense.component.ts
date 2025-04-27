import { trigger, transition, style, animate, state } from '@angular/animations';
import { Component, Inject, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { KeycloakService } from '@core/authentication/keycloak.service';
import { County, LocationData, Subcounty, Ward } from 'app/models/location.model';
import { AdmissionStatus, Contact, Relationship, Student } from 'app/models/student.model';
import { LocationService } from 'app/service/location.service';
import { StudentService } from 'app/service/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FinanceMpesaStkPushComponent } from 'app/routes/finance/mpesa-stk-push/mpesa-stk-push.component';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Expense } from 'app/models/finance.model';


@Component({
    selector: 'app-add-expense',
    templateUrl: './add-expense.component.html',
    styleUrls: ['./add-expense.component.scss']
})

export class AddExpenseComponent {

    private readonly locationService = inject(LocationService);
    private readonly studentService = inject(StudentService);
    private readonly keycloakService = inject(KeycloakService);

    expense: any = {
        description: '',
        amount: null,
        payment_reference: '',
        date_paid: '',
        reciept: null
      };

    hasSecondaryContact = false;
    studentContact: Expense[] | undefined;
    selectedFile: File | null = null;


    constructor(
        public dialogRef: MatDialogRef<AddExpenseComponent>) {
    }

    onFileSelected(event: any) {
      const file = event.target.files[0];
      if (file) {
        this.selectedFile = file;
        // You can also preview if it's an image
        console.log('Selected file:', file);
      }
    }
  
    onSubmit() {
      const formData = new FormData();
      formData.append('description', this.expense.description);
      formData.append('amount', this.expense.amount);
      formData.append('payment_reference', this.expense.payment_reference);
      formData.append('date_paid', this.expense.date_paid);
      if (this.selectedFile) {
        formData.append('reciept', this.selectedFile);
      }
  
      // TODO: send formData to your API
      console.log('Submitting expense:', formData);
  
      // Example: this.expenseService.createExpense(formData).subscribe(...)
    }
  
    cancel() {
      // Navigate away or reset form
    }
    async ngOnInit() {


    }


    closeDialog(): void {
        this.dialogRef.close();
    }
}
