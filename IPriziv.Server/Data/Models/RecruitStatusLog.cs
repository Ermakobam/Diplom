using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Log")]
    public class RecruitStatusLog
    {
        [Key]
        public int Id { get; private set; }
        [Column("Event")]
        public DateTime CreatedAt { get; private set; }
        [Column("ID_State")]
        public byte State { get; private set; }
        [Column("ID_ComForm"), ForeignKey("GatheredSubTeam")]
        public short? GatheredSubTeamId { get; private set; }
        [Column("ID_Out"), ForeignKey("Dispatching")]
        public short? TeamDispatchingId { get; private set; }
        [Column("ID_People"), ForeignKey("Recruit")]
        public int RecruitId { get; private set; }
        [Column("UserName")]
        public string UserName { get; private set; }



        [Column("ID_ROSTO"), ForeignKey("MilitaryDebtSpeciality")]
        public byte? MilitaryDebtSpecialityId { get; private set; }
        [Column("FateStr")]
        public string Assignment { get; private set; }



        public Recruit Recruit { get; private set; }
        public TeamDispatching Dispatching { get; private set; }
        public GatheredSubTeam GatheredSubTeam { get; private set; }
        public RecruitStatusLog(byte state, string userName, short? gatheredSubTeamId = null, short? teamDispatchingId = null)
        {
            State = state;
            GatheredSubTeamId = gatheredSubTeamId;
            TeamDispatchingId = teamDispatchingId;
            UserName = userName;
            CreatedAt = DateTime.Now;
        }
    }
}
