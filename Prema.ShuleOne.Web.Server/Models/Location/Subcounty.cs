using Prema.ShuleOne.Web.Server.Models.BaseTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Prema.ShuleOne.Web.Server.Models.Location
{
    [Table("subcounty")]
    public class Subcounty : BaseType
    {
        [Required]
        public int fk_county_id { get; set; }
        [ForeignKey("fk_county_id")]
        public County County { get; set; }

        public ICollection<Ward> Wards { get; set; }
    }

    public class SubcountyDto : BaseType
    {
    }
}
