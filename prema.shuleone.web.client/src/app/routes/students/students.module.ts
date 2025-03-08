import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { StudentsRoutingModule } from './students-routing.module';
import { StudentsAdmissionComponent } from './admission/admission.component';
import { StudentsAcademicsComponent } from './academics/academics.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { AdmissionFormComponent } from './admission/admission_form/admission-form.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { StudentsStudentRecordsComponent } from './student-records/student-records.component';
import { ContactInfoComponent } from './student-records/contact_info/contact-info.component';


const COMPONENTS: any[] = [StudentsAdmissionComponent, StudentsAcademicsComponent, AdmissionFormComponent, StudentsStudentRecordsComponent, ContactInfoComponent];
const COMPONENTS_DYNAMIC: any[] = [];

@NgModule({
  imports: [
    SharedModule,
    StudentsRoutingModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  declarations: [
    ...COMPONENTS,
    ...COMPONENTS_DYNAMIC
  ]
})
export class StudentsModule { }
