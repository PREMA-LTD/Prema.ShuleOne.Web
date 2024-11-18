import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StudentsAdmissionComponent } from './admission/admission.component';
import { StudentsAcademicsComponent } from './academics/academics.component';

const routes: Routes = [
  { path: 'admission', component: StudentsAdmissionComponent },
  { path: 'academics', component: StudentsAcademicsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentsRoutingModule { }
