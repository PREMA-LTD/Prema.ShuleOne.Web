using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("employee_bank_details")]
    public class EmployeeBankDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string bank_name { get; set; }
        public string bank_branch { get; set; }
        public string account_no { get; set; }
    }
}
