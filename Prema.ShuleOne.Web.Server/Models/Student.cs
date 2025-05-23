﻿using Prema.ShuleOne.Web.Server.Models.BaseEntities;
using Prema.ShuleOne.Web.Server.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("student")]
    public class Student : Person
    {
        public Grades current_grade { get; set; }
        public DateTime date_of_admission { get; set; }
        public string upi {  get; set; }
        public string assessment_no { get; set; }
        public string birth_cert_entry_no { get; set; }
        public string medical_needs {  get; set; }
        public DateOnly date_of_birth { get; set; }
        public AdmissionStatus admission_status { get; set; }
        public ICollection<Document> Documents { get; set; }
    }

    public class StudentDto : PersonDto
    {
        public Grades current_grade { get; set; }
        public DateTime date_of_admission { get; set; }
        public string upi { get; set; }
        public string assessment_no { get; set; }
        public string birth_cert_entry_no { get; set; }
        public string medical_needs { get; set; }
        public DateOnly date_of_birth { get; set; }
        public StudentContactDto primary_contact { get; set; }
        public StudentContactDto? secondary_contact { get; set; }
    }

    public enum AdmissionStatus
    {
        Pending = 1,
        Admitted = 2,
        Transfered = 3,
        Inactive = 4
    }

    [Table("admission_letter")]
    public class AdmissionLetter 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int fk_student_id { get; set; }
        public string file_name { get; set; }
        public string file_location { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public string fk_created_by { get; set; } //fk from auth server

        [ForeignKey("fk_student_id")]
        public Student Student { get; set; }
    }
}
