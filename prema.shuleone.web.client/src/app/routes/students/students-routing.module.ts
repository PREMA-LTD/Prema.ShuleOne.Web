import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StudentsAdmissionComponent } from './admission/admission.component';
import { StudentsAcademicsComponent } from './academics/academics.component';
import { StudentsStudentRecordsComponent } from './student-records/student-records.component';

const routes: Routes = [
  { path: 'admission', component: StudentsAdmissionComponent },
  { path: 'academics', component: StudentsAcademicsComponent },
  { path: 'StudentRecords', component: StudentsStudentRecordsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentsRoutingModule { }
