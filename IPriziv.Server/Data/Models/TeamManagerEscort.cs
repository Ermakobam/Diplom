using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_TeamManagerEscort")]
    public class TeamManagerEscort
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MilitaryDocumentSeries { get; set; }
        public string MilitaryDocumentNumber { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        [DefaultValue(Sex.Male)]
        public Sex Sex { get; set; }

        [ForeignKey("MilitaryRank")]
        public byte? MilitaryRankId { get; set; }
        [ForeignKey("TeamManager")]
        public short TeamManagerId { get; set; }
        [Column("MilitaryDocumentWhen")]
        public DateTime? MilitaryDocumentIssuedAt { get; set; }
        [Column("PassportWhen")]
        public DateTime? PassportIssuedAt { get; set; }

        public MilitaryRank MilitaryRank { get; set; }
        public GatheredTeamManager TeamManager { get; set; }
    }
}
