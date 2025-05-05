export interface DateOfBirth {
    year: number;
    month: number;
    day: number;
    dayOfWeek: number;
  }

  export enum Relationship {
    Father = 0,
    Mother = 1,
    Uncle = 2,
    Aunt = 3,
    Grandfather = 4,
    Grandmother = 5,
    Brother = 6,
    Sister = 7,
    Guardian = 8,
    Other = 9
  }

  export enum Grades
  {
      PlayGroup = 10,
      PP1 = 11,
      PP2 = 12,
      One = 1,
      Two = 2,
      Three = 3,
      Four = 4,
      Five = 5,
      Six = 6,
      Seven = 7,
      Eight = 8,
      Nine = 9
  }
  
  export interface Contact {
    id: number;
    surname: string;
    other_names: string;
    date_created: string;
    date_updated: string;
    fk_created_by: string;
    gender: number;
    village_or_estate: string;
    fk_residence_ward_id: number;
    contact_priority: number;
    phone_number: string;
    email: string;
    occupation: string;
    relationship: Relationship;
  }
  
  export interface Student {
    id: number;
    surname: string;
    other_names: string;
    date_created: string;
    date_updated: string;
    fk_created_by: string;
    gender: number;
    village_or_estate: string;
    fk_residence_ward_id: number;
    current_grade: Grades;
    date_of_admission: string;
    upi: string;
    assessment_no: string;
    birth_cert_entry_no: string;
    medical_needs: string;
    date_of_birth: string;
    primary_contact: Contact;
    secondary_contact: Contact | null;
    admission_status: AdmissionStatus;
  }
  
  export enum AdmissionStatus
  {
      Pending = 1,
      Admitted = 2,
      Transfered = 3
  }
  
  export interface StudentPagination {
    total: number;
    students: Student[];
  }