using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("general_ledger")]
    public class GeneralLedger
    {
        [Key]
        public int TransactionId { get; set; }
        public int Account_fk { get; set; }
        public int TransactionType_fk { get; set; }
        public decimal Amount { get; set; }
        public string Refrence { get; set; }
        public string Description { get; set; }
        public int AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateOfTransaction { get; set; }
    }
}
