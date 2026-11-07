using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ComMVO")]
    public class ExternalTeam
    {
        [Key]
        [Column("ID_ComMVO")]
        public short Id { get; set; }
        public string Name { get; set; }
        [Column("InternalTeamId")]
        public short InternalTeamId { get; set; }
        [Column("Notes")]
        public string Note { get; set; }
        /// <summary>Текст об изменении наряда (отдельно от общего примечания).</summary>
        [Column("OrderChangeNotes")]
        public string OrderChangeNote { get; set; }
        [Column("ID_MilType")]
        public byte MilitaryTypeId { get; set; }
        [Column("ID_MilUnit")]
        public short MilitaryUnitId { get; set; }
        [Column("ID_VOSO"), MaxLength(250)]
        public string ConnectionMinistryId { get; set; }
        public bool IsDirectSubmission { get; set; }

        public List<DocumentPackage> DocumentPackages { get; set; }
        public List<GatheredSubTeam> GatheredSubTeams { get; set; }
        public List<ExternalSubTeam> SubTeams { get; set; }
        public List<ReportedSubTeam> ReportedTeams { get; set; }

        [ForeignKey("MilitaryUnitId")]
        public MilitaryUnit MilitaryUnit { get; set; }
        [ForeignKey("MilitaryTypeId")]
        public MilitaryType MilitaryType { get; set; }
    }
}
