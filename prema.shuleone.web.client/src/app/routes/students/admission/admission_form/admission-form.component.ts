import { trigger, transition, style, animate, state } from '@angular/animations';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { KeycloakService } from '@core/authentication/keycloak.service';
import { County, LocationData, Subcounty, Ward } from 'app/models/location.model';
import { AdmissionStatus, Relationship, Student } from 'app/models/student.model';
import { LocationService } from 'app/service/location.service';
import { StudentService } from 'app/service/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FinanceMpesaStkPushComponent } from 'app/routes/finance/mpesa-stk-push/mpesa-stk-push.component';
import { MatDialog } from '@angular/material/dialog';


@Component({
  selector: 'app-admission-form',
  templateUrl: './admission-form.component.html',
  styleUrls: ['./admission-form.component.scss'],
  animations: [
    trigger('stepTransition', [
      // Define initial state (void) when the element is not present
      state('void', style({ 
        transform: 'translateX(100%)',
        opacity: 0,        
        position: 'absolute',  // Ensure it is out of flow
        width: '100%'  
      })),
      // Define the active state (any other state)
      state('*', style({ 
        transform: 'translateX(0)',
        opacity: 1,
        position: 'relative',  // Optional: relative if needed
        width: '100%'   
      })),
      // Define the :enter transition (void -> *)
      transition(':enter', [
        style({
          transform: 'translateX(100%)',
          opacity: 0
        }),
        animate('300ms ease-out', style({ 
          transform: 'translateX(0)',
          opacity: 1
        }))
      ]),
      // Define the :leave transition (* -> void)
      transition(':leave', [
        animate('300ms ease-in', style({ 
          transform: 'translateX(-100%)',
          opacity: 0
        }))
      ])
    ])
  ]
  
})

export class AdmissionFormComponent {
  
  private readonly locationService = inject(LocationService);
  private readonly studentService = inject(StudentService);
  private readonly keycloakService = inject(KeycloakService);
  
  currentStep = 1;
  steps = [1, 2, 3, 4];  

  studentForm: FormGroup;
  placeOfResidence: FormGroup;
  primaryContactForm : FormGroup;
  secondaryContactForm : FormGroup;
  otherForm: FormGroup;

  isLoading: boolean = true;
  isSubcountyDisabled: boolean = true;
  isWardDisabled: boolean = true;
  counties: County[] = [];
  subcounties: Subcounty[] = [];
  wards: Ward[] = [];
  selectedCountyId: number | null = null; // Variable to store the selected county ID
  selectedSubcountyId: number | null = null; // Variable to store the selected county ID

  relationships = Object.entries(Relationship) // Convert enum to array of key-value pairs
  .filter(([key]) => isNaN(Number(key))) // Remove numeric keys
  .map(([key, value]) => ({ key, value })); // Format as array of objects

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
  
  constructor(private fb: FormBuilder, private _snackBar: MatSnackBar, public dialog: MatDialog) {
    this.studentForm = this.fb.group({
      surname: ['', Validators.required],
      otherNames: ['', Validators.required],
      grade: ['', Validators.required],
      assessmentNo: [''],
      birthCert: [''],
      upi: [''],
      dob: ['', Validators.required]
    });

    this.placeOfResidence = this.fb.group({      
      county: ['', Validators.required],
      subcounty: ['', Validators.required],
      ward: ['', Validators.required],
      villageOrEstate: ['', Validators.required]
    })

    this.primaryContactForm = this.fb.group({
      primaryContactSurname: ['', Validators.required],
      primaryContactOthernames: ['', Validators.required],
      primaryContactNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(/^0\d{9}$/) // Regex: starts with 0 and followed by 9 digits
        ]
      ],
      primaryContactRelationship: ['', Validators.required],
      primaryContactOccupation: ['', Validators.required]
    });
     
    this.secondaryContactForm = this.fb.group({
      secondaryContactSurname: [''],
      secondaryContactOthernames: [''],
      secondaryContactNumber: [
        '',
        [
          Validators.pattern(/^0\d{9}$/) // Regex: starts with 0 and followed by 9 digits
        ]
      ],
      secondaryContactRelationship: [''],
      secondaryContactOccupation: ['']
    });

    this.otherForm = this.fb.group({
      specialNeeds: [''],
      admissionDate: [new Date()]
    });

    this.placeOfResidence.get('subcounty')?.disable();
    this.placeOfResidence.get('ward')?.disable();

    this.getCounties();
  }

  getStepClass(step: number): string {
    if (step === this.currentStep) {
      return 'step-indicator step-active';
    } else if (step < this.currentStep) {
      return 'step-indicator step-completed';
    }
    return 'step-indicator step-inactive';
  }

  getLineClass(stepIndex: number): string {
    return stepIndex < this.currentStep - 1 ? 'step-line line-completed' : 'step-line line-inactive';
  }

  nextStep(): void {
    console.log("current step: "+this.currentStep)

    if (this.currentStep === 1 && this.studentForm.invalid) {
      this.studentForm.markAllAsTouched(); // Highlight all invalid fields
      this.logFormErrors(this.studentForm, 'Student Form');
      return;
    }
    
    if (this.currentStep === 2 && this.placeOfResidence.invalid) {
      this.placeOfResidence.markAllAsTouched();
      this.logFormErrors(this.placeOfResidence, 'Place of Residence');
      return;
    }
    
    if (this.currentStep === 3 && this.primaryContactForm.invalid) {
      this.primaryContactForm.markAllAsTouched();
      this.logFormErrors(this.primaryContactForm, 'Primary Contact Form');
      return;
    }
    
    if (this.currentStep === 4 && this.secondaryContactForm.invalid) {
      this.secondaryContactForm.markAllAsTouched();
      this.logFormErrors(this.secondaryContactForm, 'Primary Contact Form');
      return;
    }

    if (this.currentStep === 5 && this.otherForm.invalid) {
      this.otherForm.markAllAsTouched();
      this.logFormErrors(this.otherForm, 'Other Form');
      return;
    }
    
  
    if (this.currentStep < 5) {
      this.currentStep++;
    }
  }  

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  private logFormErrors(formGroup: FormGroup, formName: string): void {
    console.error(`Errors in ${formName}:`);
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      if (control && control.invalid) {
        console.error(`- Field: ${key}`);
        const errors = control.errors;
        if (errors) {
          Object.keys(errors).forEach((errorKey) => {
            console.error(`  - Error: ${errorKey}, Value: ${errors[errorKey]}`);
          });
        }
      }
    });
  }
  
  //#region "Location logic"
  async getLocation(wardId: number): Promise<LocationData | undefined> {
    this.isLoading = true;
    this.isSubcountyDisabled = false;
    try {
      const location = await (await this.locationService.getLocation(wardId)).toPromise();
      this.isLoading = false;
      return location;
    } catch (error) {
      console.error('Error fetching location', error);
      this.isLoading = false;
      return undefined;
    }
  }

  async getCounties() {
    this.isLoading = true;
    (await this.locationService.getCounties()).subscribe(
      (data: County[]) => {
        this.counties = data;  // Assign the data to the counties array
      },
      (error) => {
        console.error('Error fetching counties', error);
      }
    );
    this.placeOfResidence.get('county')?.setValidators([Validators.required]);
    this.placeOfResidence.get('county')?.updateValueAndValidity();
    this.isLoading = false;
  }

  async getSubcounties(countyId: number) {
    this.isLoading = true;
    (await this.locationService.getSubcounties(countyId)).subscribe(
      (data: Subcounty[]) => {
        this.subcounties = data;  // Assign the data to the counties array
      },
      (error) => {
        console.error('Error fetching subcounties', error);
      }
    );
    this.isLoading = false;
  }

  async getWards(subcountyId?: number) {
    this.isLoading = true;
    (await this.locationService.getWards(subcountyId)).subscribe(
      (data: Ward[]) => {
        this.wards = data;  // Assign the data to the counties array
      },
      (error) => {
        console.error('Error fetching wards', error);
      }
    );
    this.placeOfResidence.get('ward')?.enable();
    this.isLoading = false;
  }

  onCountyChange(countyId: number) {
    this.isLoading = true;
    this.isSubcountyDisabled = false;
    this.getSubcounties(countyId);
    this.placeOfResidence.get('subcounty')?.setValidators([Validators.required]);
    this.placeOfResidence.get('ward')?.setValidators([Validators.required]);
    this.placeOfResidence.get('subcounty')?.updateValueAndValidity();
    this.placeOfResidence.get('ward')?.updateValueAndValidity();
    this.placeOfResidence.get('subcounty')?.enable();
    this.placeOfResidence.get('ward')?.disable();
    this.isLoading = false;
  }

  onSubcountyChange(subcountyId: number) {
    this.isLoading = true;
    this.getWards(subcountyId);
    this.placeOfResidence.get('ward')?.enable();
    this.placeOfResidence.get('ward')?.setValidators([Validators.required]);
    this.placeOfResidence.get('ward')?.updateValueAndValidity();
    this.isLoading = false;
  }
  //#endregion

  async submitForm(): Promise<void> {
    this.isLoading = true;
    if (this.studentForm.invalid || this.primaryContactForm.invalid || this.secondaryContactForm.invalid || this.otherForm.invalid) {

      // Mark all forms as touched to display validation errors
      this.studentForm.markAllAsTouched();
      this.primaryContactForm.markAllAsTouched();
      this.secondaryContactForm.markAllAsTouched();
      this.otherForm.markAllAsTouched();
      return;
    }
  
    const studentData = this.studentForm.value;
    const placeOfResidenceData = this.placeOfResidence.value;
    const primaryContactData = this.primaryContactForm.value;
    const secondaryContactData = this.secondaryContactForm.value;
    const otherData = this.otherForm.value;
  
    // Proceed with submission logic
    const userId = this.keycloakService.keycloak.profile?.id;
    const dob = new Date(studentData.dob);
    const studentAdmissionData: Student = {
      id: 0,
      surname: studentData.surname || '',
      other_names: studentData.otherNames || '',
      date_created: new Date().toISOString(),
      date_updated: new Date().toISOString(),
      fk_created_by: userId || 'unknown',
      gender: studentData.gender,
      village_or_estate: placeOfResidenceData.villageOrEstate,
      fk_residence_ward_id: placeOfResidenceData.ward,
      current_grade: studentData.grade,
      date_of_admission: otherData.admissionDate,
      upi: studentData.upi,
      assessment_no: studentData.upi,
      birth_cert_entry_no: studentData.upi,
      medical_needs: otherData.specialNeeds,
      date_of_birth: `${dob.getFullYear()}-${(dob.getMonth() + 1).toString().padStart(2, '0')}-${dob.getDate().toString().padStart(2, '0')}`,
      primary_contact: {
        id: 0,
        surname: primaryContactData?.primaryContactSurname,
        other_names: primaryContactData?.primaryContactOthernames,
        date_created: new Date().toISOString(),
        date_updated: new Date().toISOString(),
        fk_created_by: userId || 'unknown',
        gender: 0,
        village_or_estate: placeOfResidenceData.villageOrEstate,
        fk_residence_ward_id: placeOfResidenceData.ward,
        contact_priority: 1,
        phone_number: primaryContactData?.primaryContactNumber,
        email: '',
        occupation: primaryContactData?.primaryContactOccupation,
        relationship: primaryContactData?.primaryContactRelationship
      },
      secondary_contact: secondaryContactData?.secondaryContactSurname == "" ? null : {
        id: 0,
        surname: secondaryContactData?.secondaryContactSurname,
        other_names: secondaryContactData?.secondaryContactOthernames,
        date_created: new Date().toISOString(),
        date_updated: new Date().toISOString(),
        fk_created_by: userId || 'unknown',
        gender: 0,
        village_or_estate: placeOfResidenceData.villageOrEstate,
        fk_residence_ward_id: placeOfResidenceData.ward,
        contact_priority: 2,
        phone_number: secondaryContactData?.secondaryContactNumber,
        email: '',
        occupation: secondaryContactData?.secondaryContactOccupation,
        relationship: secondaryContactData?.secondaryContactRelationship
      },
      admission_status: AdmissionStatus.Pending
    };

    console.log('studentAdmissionData:', studentAdmissionData);

    //send to api
    try{
    const response = await this.studentService.admitStudent(studentAdmissionData)
    
      if(response){
      this._snackBar.open('Student admitted successfully.', 'Ok', {
        horizontalPosition: 'right',
        verticalPosition: 'top',
        duration: 5 * 1000,
        });
        const paymentDetails: any = {
          feeType: "Admission Fee",
          amount: 2,
          mpesaNumber: studentAdmissionData.primary_contact.phone_number
        }

        this.admitNewStudent(paymentDetails);
        
      } else {
        this._snackBar.open('Error admitting student, please try again.', 'Ok', {
          panelClass: ['error-snackbar'],  // Add a custom CSS class
          horizontalPosition: 'right',
          verticalPosition: 'top',
          duration: 5 * 1000,
          });
      }

    
    } catch(error) {
      console.error('Error sending message:', error);

      
      this._snackBar.open('Error admitting student, please try again.', 'Ok', {
        panelClass: ['error-snackbar'],  // Add a custom CSS class
        horizontalPosition: 'right',
        verticalPosition: 'top',
        duration: 5 * 1000,
        });
    };

    this.isLoading = false;
  }

  admitNewStudent(paymentData: any){
    console.log("admitNewStudent")
    const dialogRef = this.dialog.open(FinanceMpesaStkPushComponent, {
      width: '400px',
      data: { paymentData }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.dialog.closeAll();
    });
  }

  close() {
    this.dialog.closeAll(); 
  }
}
