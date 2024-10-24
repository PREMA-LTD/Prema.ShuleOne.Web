using Prema.ShuleOne.Web.Server.Models.BaseEntities;
using Prema.ShuleOne.Web.Server.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("student")]
    public class Student : Person
    {
        public Grade current_grade { get; set; }
        public DateTime date_of_admission { get; set; }
        public string upi {  get; set; }
        public string assessment_no { get; set; }
        public string birth_cert_entry_no { get; set; }
        public string medical_needs {  get; set; }
        public DateOnly date_of_birth { get; set; }

    }
}
