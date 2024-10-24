using Prema.ShuleOne.Web.Server.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Prema.ShuleOne.Web.Server.Models.Location;
using Microsoft.EntityFrameworkCore;

namespace Prema.ShuleOne.Web.Server.Models.BaseEntities
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string surname { get; set; }
        public string other_names { get; set; }
        public DateTime date_created  { get; set; }
        public DateTime date_updated { get; set; }
        public string fk_created_by {  get; set; } //fk from auth server
        public Gender gender { get; set; }                
        public string village_or_estate { get; set; }

        [Required]
        public int fk_residence_ward_id { get; set; }
        [ForeignKey("fk_residence_ward_id")]
        public Ward Ward { get; set; }

    }

    public class PersonDto
    {
        public int id { get; set; }
        public string surname { get; set; }
        public string other_names { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public string fk_created_by { get; set; } //fk from auth server
        public Gender gender { get; set; }
        public string village_or_estate { get; set; }
        public int fk_residence_ward_id { get; set; }

    }
}
