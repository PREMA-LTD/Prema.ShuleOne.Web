using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("document")]
    public class Document
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public DocumentType document_type { get; set; }
        public string file_name { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public string fk_created_by { get; set; } //fk from auth server for docs should default to system


        [Required]
        public int fk_student_id { get; set; }
        [ForeignKey("fk_student_id")]
        public Student Student { get; set; }
    }

    public enum DocumentType
    {
        AdmissionLetter,
        ProgressReport,
        Certificate
    }

}
