using Prema.ShuleOne.Web.Server.Models.BaseTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models.Location
{
    [Table("county")]
    public class County : BaseType
    {

        public ICollection<Subcounty> Subcounties { get; set; }
    }

    public class CountyDto : BaseType
    {
    }
}
