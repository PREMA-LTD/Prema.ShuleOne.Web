using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("sms_failure")]
    public class SMSFailure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string error { get; set; }
        public DateTime date_time { get; set; }


        [Required]
        public int fk_sms_record_id { get; set; }
        [ForeignKey("fk_sms_record_id")]
        public SMSRecord SMSRecord { get; set; }
    }
}
