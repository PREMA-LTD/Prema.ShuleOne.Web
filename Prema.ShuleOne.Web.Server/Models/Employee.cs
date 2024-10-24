using Prema.ShuleOne.Web.Server.Models.BaseEntities;
using Prema.ShuleOne.Web.Server.Models.Location;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("employee")]
    public class Employee : Person
    {
        public string phone_number { get; set; }
        public string? email { get; set; }
        public string? kra { get; set; }
        public string shif { get; set; }
        public string nssf { get; set; }

        [Required]
        public int fk_employee_bank_details { get; set; }
        [ForeignKey("fk_employee_bank_details")]
        public EmployeeBankDetails EmployeeBankDetails { get; set; }
    }
}
