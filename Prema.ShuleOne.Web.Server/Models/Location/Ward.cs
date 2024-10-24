using Prema.ShuleOne.Web.Server.Models.BaseTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Prema.ShuleOne.Web.Server.Models.BaseEntities;

namespace Prema.ShuleOne.Web.Server.Models.Location
{
    [Table("ward")]
    public class Ward : BaseType
    {

        [Required]
        public int fk_subcounty_id { get; set; }
        [ForeignKey("fk_subcounty_id")]
        public Subcounty Subcounty { get; set; }

        public ICollection<Student> Student { get; set; }
        public ICollection<Employee> Employee { get; set; }
        public ICollection<StudentContact> StudentContact { get; set; }
    }

    public class WardDto : BaseType
    {
    }
}
