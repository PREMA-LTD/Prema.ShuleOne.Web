import { trigger, transition, style, animate, state } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';


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
  currentStep = 1;
  steps = [1, 2, 3];  

  studentForm: FormGroup;
  primaryContactForm : FormGroup;
  secondaryContactForm : FormGroup;
  otherForm: FormGroup;

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
      fullName: [''],
      grade: [''],
      dob: ['']
    });

    this.primaryContactForm = this.fb.group({
      primaryParentName: ['', Validators.required],
      primaryContactNumber: ['', Validators.required],
      primaryRelationship: ['', Validators.required]
    });
  
    this.secondaryContactForm = this.fb.group({
      secondaryParentName: ['', Validators.required],
      secondaryContactNumber: ['', Validators.required],
      secondaryRelationship: ['', Validators.required]
    });

    this.otherForm = this.fb.group({
      remarks: [''],
      admissionDate: ['']
    });
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
    if (this.currentStep < 3) {
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  submitForm() {
    const studentData = this.studentForm.value;
    const primaryContactForm = this.primaryContactForm.value;
    const secondaryContactForm  = this.secondaryContactForm .value;
    const otherData = this.otherForm.value;

    console.log('Student Data:', studentData);
    console.log('Parent Data:', primaryContactForm);
    console.log('Parent Data:', secondaryContactForm);
    console.log('Other Data:', otherData);
  }

}
