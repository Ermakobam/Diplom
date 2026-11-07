using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Wordprocessing;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ComMVOPart")]
    public class ExternalSubTeam
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime OutDate { get; set; }
        [Column("AddReq")]
        public string AdditionalRequirements { get; set; }
        [Column("Number")]
        public short SendPlanCount { get; set; }
        public bool IsEducational { get; set; }
        public bool IsSecretCommunication { get; set; }


        [Column("ID_ComMVO"), ForeignKey("ExternalTeam")]
        public short ExternalTeamId { get; set; }
        [Column("ID_ComGSh"), ForeignKey("Regime")]
        public byte? RegimeId { get; set; }
        [Column("ID_ROSTO"), ForeignKey("MilitaryDebtSpeciality")]
        public byte? MilitaryDebtSpecialityId { get; set; }
        [Column("ID_Admiss"), ForeignKey("Admiss")]
        public byte? AdmissId { get; set; }
        [Column("ID_Health"), ForeignKey("HealthCategory")]
        public byte? HealthId { get; set; }
        [ForeignKey("CivilSpeciality")]
        public int? CivilSpecialityId { get; set; }
        [ForeignKey("UniformColor")]
        public int? UniformColorId { get; set; }


        public UniformColor UniformColor { get; set; }
        public CivilSpeciality CivilSpeciality { get; set; }
        public ExternalTeam ExternalTeam { get; set; }
        public MilitaryDebtSpeciality MilitaryDebtSpeciality { get; set; }
        public Admiss Admiss { get; set; }
        public HealthCategory HealthCategory { get; set; }
        public ExternalTeamRegime Regime { get; set; }

        public List<ReportedSubTeam> ReportedSubTeams { get; set; }
    }
}
