using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models.Dictionaries;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ComForm")]
    public class GatheredSubTeam
    {
        [Key]
        [Column("ID_ComForm")]
        public short Id { get; private set; }
        [Required(AllowEmptyStrings = false), MaxLength(50)]
        public string Assignment { get; private set; }
        [MaxLength(500)]
        public string Note { get; private set; }

        [Column("FormDate"), DataType("smalldatetime")]
        public DateTime CreatedAt { get; private set; }

        public int DocsOnModule { get; private set; }
        public int MilDocs { get; private set; }
        public int RecruitsOnClothes { get; private set; }
        public int DocsOnBanks { get; private set; }
        public bool HasNameLists { get; private set; }
        public bool GiveNameLists { get; private set; }


        [Column("ID_ComMVO"), ForeignKey("ExternalTeam")]
        public short ExternalTeamId { get; private set; }

        [ForeignKey("UniformColor")]
        public int? UniformColorId { get; private set; }



        public UniformColor UniformColor
        {
            get => _loader.Load(this, ref _uniformColor);
            private set => _uniformColor = value;
        }
        public ExternalTeam ExternalTeam
        {
            get => _loader.Load(this, ref _externalTeam);
            private set => _externalTeam = value;
        }
        public TeamDispatching TeamDispatching
        {
            get => _loader.Load(this, ref _teamDispatching);
            private set => _teamDispatching = value;
        }



        public List<Recruit> Recruits
        {
            get => _loader.Load(this, ref _recruits);
            private set => _recruits = value;
        }
        public List<RecruitStatusLog> RecruitStatusLogs
        {
            get => _loader.Load(this, ref _recruitStatusLogs);
            private set => _recruitStatusLogs = value;
        }
        public List<Overnight> Overnights
        {
            get => _loader.Load(this, ref _overnights);
            private set => _overnights = value;
        }
        public List<DispatchingSendPlan> DispatchingSendPlans
        {
            get => _loader.Load(this, ref _dispatchingSendPlans);
            private set => _dispatchingSendPlans = value;
        }


        private readonly ILazyLoader _loader;

        private UniformColor _uniformColor;
        private ExternalTeam _externalTeam;
        private TeamDispatching _teamDispatching;

        private List<Recruit> _recruits;
        private List<RecruitStatusLog> _recruitStatusLogs;
        private List<Overnight> _overnights;
        private List<DispatchingSendPlan> _dispatchingSendPlans;

        [SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "Необходимо для работы отложенной загрузки")]
        private GatheredSubTeam(ILazyLoader loader)
        {
            _loader = loader;
        }
        public GatheredSubTeam(short externalTeamId, string assignment, int uniformColorId)
        {
            Assignment = assignment;
            Note = null;
            CreatedAt = DateTime.Now;
            ExternalTeamId = externalTeamId;
            UniformColorId = uniformColorId;
        }
        public void SetNote(string note)
        {
            Note = note;
        }
        public void Edit(string assignment, int uniformColorId)
        {
            Assignment = assignment;
            UniformColorId = uniformColorId;
        }

        public void SetCounters(int docs, int mildocs, int clothes, int banks)
        {

            DocsOnModule = docs;
            MilDocs = mildocs;
            RecruitsOnClothes = clothes;
            DocsOnBanks = banks;

        }

        public void GettingNameLists()
        {

            HasNameLists = !HasNameLists;

        }
        public void GivingNameLists()
        {

            GiveNameLists = !GiveNameLists;

        }

    }
}
