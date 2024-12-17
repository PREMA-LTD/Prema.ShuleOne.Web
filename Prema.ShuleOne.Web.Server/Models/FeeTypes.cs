using Prema.ShuleOne.Web.Server.Models.BaseTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("fee_type")]
    public class FeeType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public string name { get; set; }
    }

    public enum FeeTypes
    {
        Other = 0,
        Tution = 1,
        Transport = 2,
        Remidial = 3, 
        Trip = 4,
        Uniform = 5,
        PocketMoney = 6
    }
}
