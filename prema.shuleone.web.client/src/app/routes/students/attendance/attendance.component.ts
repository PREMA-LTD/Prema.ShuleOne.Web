import { Component, OnInit, OnDestroy } from '@angular/core';

import { PageEvent } from '@angular/material/paginator'; // Add this import
import { MatRadioChange } from '@angular/material/radio';
export interface Student {
  id: number;
  name: string;
  rollNumber: string;
  attendance: 'present' | 'absent' | null;
}
@Component({
  selector: 'app-students-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss']
})

export class StudentsAttendanceComponent implements OnInit {

  students: Student[] = [
    {
      id: 1,
      name: 'Samuel Gitongo',
      rollNumber: '17',
      attendance: null
    },
    {
      id: 2,
      name: 'Sifan Getahun',
      rollNumber: '73',
      attendance: null
    },
    {
      id: 3,
      name: 'Sunny Spears',
      rollNumber: '24',
      attendance: null
    },
    {
      id: 4,
      name: 'Sylvia Tanui',
      rollNumber: '67',
      attendance: null
    },
    {
      id: 5,
      name: 'John Doe',
      rollNumber: '45',
      attendance: null
    },
    {
      id: 6,
      name: 'Jane Smith',
      rollNumber: '32',
      attendance: null
    }
  ];

  constructor() { }

  ngOnInit(): void {
  }

  setAttendance(studentId: number, event: MatRadioChange | any): void {
    const student = this.students.find(s => s.id === studentId);
    if (student && event.value) {
      student.attendance = event.value;
    }
  }


  setAllPresent(): void {
    this.students.forEach(student => {
      student.attendance = 'present';
    });
  }

  saveAttendance(): void {
    // TODO: Implement save logic
    console.log('Save attendance:', this.students);
  }

  getPresentCount(): number {
    return this.students.filter(s => s.attendance === 'present').length;
  }

  getAbsentCount(): number {
    return this.students.filter(s => s.attendance === 'absent').length;
  }

  getNextPage(event: PageEvent): void {
    // Your pagination logic here
    console.log('Page event:', event);
    console.log('Page index:', event.pageIndex);
    console.log('Page size:', event.pageSize);
    console.log('Length:', event.length);
  }
}
