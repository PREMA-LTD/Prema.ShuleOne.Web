using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("tution_fees_structure")]
    public class TutionFeesStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int term { get; set; }
        public decimal amount { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public string updated_by { get; set; }

        [ForeignKey("fk_grade_id")]
        public Grade grade { get; set; }
    }
}
