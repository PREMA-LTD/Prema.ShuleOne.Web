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
import { Expense, ExpenseCategoryDto, ExpenseDto, ExpenseSubCategoryDto } from 'app/models/finance.model';
import { AccountingService } from 'app/service/accounting.service';


@Component({
    selector: 'app-add-expense',
    templateUrl: './add-expense.component.html',
    styleUrls: ['./add-expense.component.scss']
})

export class AddExpenseComponent {

  addExpenseForm: FormGroup;

  constructor(public dialogRef: MatDialogRef<AddExpenseComponent>, private fb: FormBuilder){
    this.addExpenseForm = this.fb.group({
      description: ['', Validators.required],
      fk_expense_subcategory_id: ['', Validators.required],
      amount: ['', Validators.required],
      payment_reference: ['', Validators.required],
      date_paid: ['', Validators.required]
    });
  }
    private readonly accountingService = inject(AccountingService);

    expense: Expense = {
      description: '',
      amount: 0,
      fk_expense_subcategory_id: 0,
      payment_reference: '',
      fk_from_account_id: 0,
      fk_to_account_id: 0,
      paid_by: '',
      date_paid: undefined,
      date_created: new Date(),
      reciept: ''
    };

      

    hasSecondaryContact = false;
    studentContact: Expense[] | undefined;
    selectedFile: File | null = null;
    expenseCategories: ExpenseCategoryDto[] | undefined;
    expenseSubCategories: ExpenseSubCategoryDto[] | undefined;


   
    onFileSelected(event: any) {
      const file = event.target.files[0];
      if (file) {
        this.selectedFile = file;
        // You can also preview if it's an image
        console.log('Selected file:', file);
      }
    }
  
    async onSubmit() {
      // Create a FormData object for multipart/form-data submission
      const formData = new FormData();

      console.log('Form data:', JSON.stringify(this.expense));
      
      // Append all the expense properties as form fields
      formData.append('id', '0'); // Assuming 0 is the default for new items
      formData.append('name', ''); // If this is required by your model
      formData.append('description', this.expense.description);
      formData.append('amount', this.expense.amount.toString());
      formData.append('fk_expense_subcategory_id', this.expense.fk_expense_subcategory_id.toString());
      formData.append('payment_reference', this.expense.payment_reference || '');
      formData.append('fk_from_account_id', this.expense.fk_from_account_id?.toString() || '0');
      formData.append('fk_to_account_id', this.expense.fk_to_account_id?.toString() || '0');
      
      // Handle nullable fk_transaction_id
      if (this.expense.fk_transaction_id) {
        formData.append('fk_transaction_id', this.expense.fk_transaction_id.toString());
      }
      
      formData.append('paid_by', this.expense.paid_by || '');
            
      formData.append('date_paid', this.formatDateForBackend(this.expense.date_paid || new Date()));
      formData.append('date_created', this.formatDateForBackend(new Date()));
      
      // Append the file last
      if (this.selectedFile) {
        formData.append('reciept', this.selectedFile, this.selectedFile.name);
      }
      
      try {
        // Call the service method that uses FormData
        const response = await this.accountingService.createExpense(formData);
        console.log('Expense created successfully:', response);
        this.dialogRef.close();
        // Handle success (redirect, show message, etc.)
      } catch (error) {
        console.error('Failed to create expense:', error);
        // Handle error
      }
    }
    cancel() {
      // Navigate away or reset form
    }

    async ngOnInit() {
      this.expenseCategories = await (await this.accountingService.getExpenseCategories()).toPromise();
    }

    closeDialog(): void {
        this.dialogRef.close();
    }

    onCategoryChange(expenseCategory: number) {
      this.expenseSubCategories = this.expenseCategories?.find(category => category.id === expenseCategory)?.expenseSubCategories;
      this.addExpenseForm.get('subCategory')?.setValidators([Validators.required]);
      this.addExpenseForm.get('subCategory')?.valid;
      this.addExpenseForm.get('subCategory')?.enable();
    }  

    // Convert date to ISO 8601 format that ASP.NET Core can parse properly
    formatDateForBackend(date: Date | string): string {
      if (!date) {
        return new Date().toISOString();
      }
      
      if (date instanceof Date) {
        return date.toISOString(); // Returns format: "2025-05-01T00:00:00.000Z"
      }
      
      // If it's already a string but in wrong format, try to parse and convert
      try {
        const parsedDate = new Date(date);
        if (!isNaN(parsedDate.getTime())) {
          return parsedDate.toISOString();
        }
      } catch (e) {
        console.error('Invalid date format:', e);
      }
      
      // Fallback - not recommended but prevents errors
      return new Date().toISOString();
    }



}
