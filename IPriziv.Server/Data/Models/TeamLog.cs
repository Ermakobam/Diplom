using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models
{
    [Table("T_TeamLog")]
    public class TeamLog
    {
        [Key]
        public int Id { get; private set; }
        public bool IsChecked { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string UserName { get; private set; }
        public short? InternalTeamId { get; set; }
        public short? OldInternalTeamId { get; set; }
        public short? GatheredSubTeamId { get; set; }
        public short? OldGatheredSubTeamId { get; set; }

        [ForeignKey("Recruit")]
        public int RecruitId { get; private set; }

        public Recruit Recruit { get; private set; }

        public TeamLog()
        {


        }
        public TeamLog(int recruitId, string userName, short? gatheredSubTeamId = null, short? oldGatheredSubTeamId = null, bool check = false)
        {
            RecruitId = recruitId;
            GatheredSubTeamId = gatheredSubTeamId;
            OldGatheredSubTeamId = oldGatheredSubTeamId;
            IsChecked = check;
            UserName = userName;
            CreatedAt = DateTime.Now;
        }

        public void Edit(int recruitId, string userName, short? gatheredSubTeamId = null, short? oldGatheredSubTeamId = null, short? internalTeamId = null, short? oldInternalTeamId = null, bool check = false)
        {

            RecruitId = recruitId;
            GatheredSubTeamId = gatheredSubTeamId;
            OldGatheredSubTeamId = oldGatheredSubTeamId;
            InternalTeamId = internalTeamId;
            OldInternalTeamId = oldInternalTeamId;
            IsChecked = check;
            UserName = userName;
        }

    }
}
