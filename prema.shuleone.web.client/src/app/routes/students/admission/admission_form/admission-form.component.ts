import { trigger, transition, style, animate, state } from '@angular/animations';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { County, LocationData, Subcounty, Ward } from 'app/models/location.model';
import { LocationService } from 'app/service/location.service';


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
  
  currentStep = 1;
  steps = [1, 2, 3, 4];  

  studentForm: FormGroup;
  placeOfResidence: FormGroup;
  primaryContactForm : FormGroup;
  secondaryContactForm : FormGroup;
  otherForm: FormGroup;

  isSubcountyDisabled: boolean = true;
  isWardDisabled: boolean = true;
  counties: County[] = [];
  subcounties: Subcounty[] = [];
  wards: Ward[] = [];
  selectedCountyId: number | null = null; // Variable to store the selected county ID
  selectedSubcountyId: number | null = null; // Variable to store the selected county ID


  grades = [
    { value: 1, name: 'Play Group' },
    { value: 2, name: 'PP1' },
    { value: 3, name: 'PP2' },
    { value: 4, name: 'Grade 1' },
    { value: 5, name: 'Grade 2' },
    { value: 6, name: 'Grade 3' }
  ];  
  
  constructor(private fb: FormBuilder) {
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
    })

    this.primaryContactForm = this.fb.group({
      primaryContactName: ['', Validators.required],
      primaryContactNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(/^0\d{9}$/) // Regex: starts with 0 and followed by 9 digits
        ]
      ],
      primaryContactRelationship: ['', Validators.required]
    });
  
    this.secondaryContactForm = this.fb.group({
      secondaryContactName: [''],
      secondaryContactNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(/^0\d{9}$/) // Regex: starts with 0 and followed by 9 digits
        ]
      ],
      secondaryContactRelationship: ['']
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
    if (this.currentStep === 1 && this.studentForm.invalid) {
      this.studentForm.markAllAsTouched(); // Highlight all invalid fields
      return;
    }
  
    if (this.currentStep === 2 && this.placeOfResidence.invalid) {
      this.placeOfResidence.markAllAsTouched();
      return;
    }

    if (this.currentStep === 3 && this.primaryContactForm.invalid) {
      this.primaryContactForm.markAllAsTouched();
      return;
    }
  
    if (this.currentStep === 4 && this.secondaryContactForm.invalid) {
      this.secondaryContactForm.markAllAsTouched();
      return;
    }
  
    if (this.currentStep < 3) {
      this.currentStep++;
    }
  }
  

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  //#region "Location logic"
  async getLocation(wardId: number): Promise<LocationData | undefined> {
    this.isSubcountyDisabled = false;
    try {
      const location = await (await this.locationService.getLocation(wardId)).toPromise();
      return location;
    } catch (error) {
      console.error('Error fetching location', error);
      return undefined;
    }
  }

  async getCounties() {
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
  }

  async getSubcounties(countyId: number) {
    (await this.locationService.getSubcounties(countyId)).subscribe(
      (data: Subcounty[]) => {
        this.subcounties = data;  // Assign the data to the counties array
      },
      (error) => {
        console.error('Error fetching subcounties', error);
      }
    );
  }

  async getWards(subcountyId?: number) {
    (await this.locationService.getWards(subcountyId)).subscribe(
      (data: Ward[]) => {
        this.wards = data;  // Assign the data to the counties array
      },
      (error) => {
        console.error('Error fetching wards', error);
      }
    );
    this.placeOfResidence.get('ward')?.enable();
  }

  onCountyChange(countyId: number) {
    this.isSubcountyDisabled = false;
    this.getSubcounties(countyId);
    this.placeOfResidence.get('subcounty')?.setValidators([Validators.required]);
    this.placeOfResidence.get('ward')?.setValidators([Validators.required]);
    this.placeOfResidence.get('subcounty')?.updateValueAndValidity();
    this.placeOfResidence.get('ward')?.updateValueAndValidity();
    this.placeOfResidence.get('subcounty')?.enable();
    this.placeOfResidence.get('ward')?.disable();
  }

  onSubcountyChange(subcountyId: number) {
    this.getWards(subcountyId);
    this.placeOfResidence.get('ward')?.enable();
    this.placeOfResidence.get('ward')?.setValidators([Validators.required]);
    this.placeOfResidence.get('ward')?.updateValueAndValidity();
  }
  //#endregion

  submitForm(): void {
    if (this.studentForm.invalid || this.primaryContactForm.invalid || this.secondaryContactForm.invalid || this.otherForm.invalid) {

      // Mark all forms as touched to display validation errors
      this.studentForm.markAllAsTouched();
      this.primaryContactForm.markAllAsTouched();
      this.secondaryContactForm.markAllAsTouched();
      this.otherForm.markAllAsTouched();
      return;
    }
  
    const studentData = this.studentForm.value;
    const primaryContactData = this.primaryContactForm.value;
    const secondaryContactData = this.secondaryContactForm.value;
    const otherData = this.otherForm.value;
  
    console.log('Student Data:', studentData);
    console.log('Primary Contact Data:', primaryContactData);
    console.log('Secondary Contact Data:', secondaryContactData);
    console.log('Other Data:', otherData);
  
    // Proceed with submission logic
  }
  

}
