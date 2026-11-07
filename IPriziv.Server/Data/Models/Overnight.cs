using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Overnight")]
    public class Overnight
    {
        [Key]
        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime CreatedAt { get; private set; }

        [ForeignKey("GatheredSubTeam")]
        public short? GatheredSubTeamId { get; private set; }
        [ForeignKey("ExternalTeam")]
        public short? ExternalTeamId { get; private set; }
        public ExternalTeam ExternalTeam { get; private set; }
        public List<Overnighter> Overnighters
        {
            get => _loader.Load(this, ref _overnighters);
            private set => _overnighters = value;
        }
        public GatheredSubTeam GatheredSubTeam
        {
            get => _loader.Load(this, ref _gatheredSubTeam);
            private set => _gatheredSubTeam = value;
        }

        private readonly ILazyLoader _loader;

        private GatheredSubTeam _gatheredSubTeam;
        private List<Overnighter> _overnighters;

        [SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "Необходимо для работы отложенной загрузки")]
        private Overnight(ILazyLoader loader)
        {
            _loader = loader;
        }
        public Overnight(DateTime date, short gatheredSubTeamId, short externalTeamId, List<Overnighter> overnighters)
        {
            Date = date;
            GatheredSubTeamId = gatheredSubTeamId;
            ExternalTeamId = externalTeamId;
            CreatedAt = DateTime.Now;
            _overnighters = overnighters;
        }
        public void SynchronizeRecruits(IEnumerable<int> recruitIds)
        {
            var overnighterRecruitIds = Overnighters.Where(x => x.RecruitId != null).Select(x => x.RecruitId.Value);
            var recruitIdsToRemove = overnighterRecruitIds.Except(recruitIds);
            var recreuitIdsToAdd = recruitIds.Except(overnighterRecruitIds);

            _overnighters.RemoveAll(x => x.RecruitId == null || recruitIdsToRemove.Contains(x.RecruitId.Value));

            foreach (var recruitIdsToAdd in recreuitIdsToAdd)
            {
                var newOvernighter = GatheredSubTeam.Recruits.Where(x => x.Id == recruitIdsToAdd).First();
                var overnighter = new Overnighter
                {
                    FirstName = newOvernighter.FirstName,
                    MiddleName = newOvernighter.MiddleName,
                    LastName = newOvernighter.LastName,
                    BirthDate = newOvernighter.BirthDate,
                    RecruitId = recruitIdsToAdd,
                    MilitaryOfficeId = newOvernighter.MilitaryOfficeId
                };
                Overnighters.Add(overnighter);
            }
        }
    }
}
