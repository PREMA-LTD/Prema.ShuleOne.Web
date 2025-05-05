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
    selector: 'app-expense-details',
    templateUrl: './expense-details.component.html',
    styleUrls: ['./expense-details.component.scss']
})

export class ExpenseDetailsComponent {

    private readonly locationService = inject(LocationService);
    private readonly studentService = inject(StudentService);
    private readonly keycloakService = inject(KeycloakService);

    expenseDetails: Expense;

    hasSecondaryContact = false;

    studentContact: Expense[] | undefined;

    constructor(
        public dialogRef: MatDialogRef<FinanceMpesaStkPushComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
        this.expenseDetails = data.expenseDetails;
    }

    async ngOnInit() {


    }


    closeDialog(): void {
        this.dialogRef.close();
    }
}
