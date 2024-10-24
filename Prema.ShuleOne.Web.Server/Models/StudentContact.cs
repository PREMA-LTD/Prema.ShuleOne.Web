using Prema.ShuleOne.Web.Server.Models.BaseEntities;
using Prema.ShuleOne.Web.Server.Models.Enum;
using Prema.ShuleOne.Web.Server.Models.Location;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("student_contact")]
    public class StudentContact : Person
    {
        public byte contact_priority { get; set; }
        public string phone_number { get; set; }
        public string? email { get; set; }
        public string? occupation { get; set; }
        public Relationship relationship { get; set; }


        [Required]
        public int fk_student_id { get; set; }
        [ForeignKey("fk_student_id")]
        public Student Student { get; set; }

    }
}
