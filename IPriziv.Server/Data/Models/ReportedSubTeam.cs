using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ReportedTeams")]
    public class ReportedSubTeam
    {
        [Key, Column("ID_ReportedTeams")]
        public int Id { get; set; }
        [Column("ReportDate")]
        public DateTime CreatedAt { get; set; }
        [Column("SentDate")]
        public DateTime SentAt { get; set; }
        [Column("ArriveDate")]
        public DateTime ArrivedAt { get; set; }
        [Column("TeamState")]
        public ReportTeamState State { get; set; }
        [Column("OmuNumberOne")]
        public int ExternalTeamNamePart_0 { get; set; }
        [Column("OmuNumberTwo")]
        public int ExternalTeamNamePart_1 { get; set; }
        [Column("OmuNumberThree")]
        public string ExternalTeamNamePart_2 { get; set; }
        [Column("TotalPeople")]
        public int TotalRecruits { get; set; }
        public int TotalDriversC { get; set; }
        public int TotalDriversD { get; set; }
        public int TotalDriversE { get; set; }
        public int TotalHighEducated { get; set; }


        [ForeignKey("ExternalSubTeam")]
        public int? ExternalSubTeamId { get; set; }
        [Column("ID_ComMvo"), ForeignKey("ExternalTeam")]
        public short? ExternalTeamId { get; set; }
        [Column("ID_MilUnit"), ForeignKey("MilitaryUnit")]
        public short MilitaryUnitId { get; set; }
        [Column("ID_MilType"), ForeignKey("MilitaryType")]
        public byte MilitaryTypeId { get; set; }
        [Column("ID_Station"), ForeignKey("Station")]
        public short? StationId { get; set; }
        [Column("ID_Out"), ForeignKey("TeamDispatching")]
        public short? TeamDispatchingId { get; set; }


        public ExternalSubTeam ExternalSubTeam { get; set; }
        public ExternalTeam ExternalTeam { get; set; }
        public MilitaryUnit MilitaryUnit { get; set; }
        public MilitaryType MilitaryType { get; set; }
        public Station Station { get; set; }
        public TeamDispatching TeamDispatching { get; set; }
    }
}
