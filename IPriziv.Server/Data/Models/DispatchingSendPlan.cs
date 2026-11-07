using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Controllers.Dictionaries;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    public class DispatchingSendPlan
    {
        [Key]
        public int Id { get; set; }
        public int Count { get; set; }

        [ForeignKey("GatheredSubTeam")]
        public short GatheredSubTeamId { get; set; }
        [ForeignKey("MilitaryDebtSpecaility")]
        public int MilitaryDebtSpecialityId { get; set; }

        public GatheredSubTeam GatheredSubTeam { get; set; }
        public MilitaryDebtSpeciality MilitaryDebtSpeciality { get; set; }
    }
}
