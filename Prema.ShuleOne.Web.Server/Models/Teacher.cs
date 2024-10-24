using Prema.ShuleOne.Web.Server.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("teacher")]
    public class Teacher : Employee
    {
        public string tsa_no { get; set; }
        public List<int> subjects { get; set; }
        public List<Grade> grades { get; set; }

        public string highest_qualification { get; set; }
        public DateTime start_date { get; set; }

    }
}
