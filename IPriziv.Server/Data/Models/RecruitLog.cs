using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;
//using IPriziv.Server.Migrations.Conscription;
using Newtonsoft.Json;

namespace IPriziv.Server.Data.Models
{
    [Table("T_RecruitLog")]
    public class RecruitLog
    {
        [Key]
        public int Id { get; private set; }
        public string Data { get; private set; }
        [NotMapped]
        public object DataObject
        {
            get
            {
                if (string.IsNullOrEmpty(Data))
                    return null;
                return JsonConvert.DeserializeObject(Data);
            }
            private set
            {
                if (value == null)
                {
                    Data = null;
                    return;
                }
                Data = JsonConvert.SerializeObject(value);
            }
        }
        public RecruitState TargetState { get; private set; }
        public bool IsAccidentCorrection { get; private set; }
        [MaxLength(150)]
        public string UserName { get; private set; }
        public DateTime CreatedAt { get; private set; }



        [ForeignKey("Recruit")]
        public int RecruitId { get; private set; }
        [ForeignKey("GatheredSubTeam")]
        public short? GatheredSubTeamId { get; set; }

        public Recruit Recruit { get; private set; }
        public GatheredSubTeam GatheredSubTeam { get; private set; }
        private RecruitLog() { }
        public RecruitLog(RecruitState state, string userName, object data = null, bool isAccidentCorrection = false, short? gatheredSubTeamId = null)
        {
            TargetState = state;
            UserName = userName;
            DataObject = data;
            IsAccidentCorrection = isAccidentCorrection;
            GatheredSubTeamId = gatheredSubTeamId;
            CreatedAt = DateTime.Now;
        }
    }
}
