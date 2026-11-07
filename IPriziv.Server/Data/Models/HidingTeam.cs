using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPriziv.Server.Data.Models
{
    [Table("T_SendPlan")]
    public class HidingTeam
    {

        [Key, Column("ID_SendPlan")]
        public int Id { get; set; }

        public DateTime OutDate { get; set; }
        public short Number { get; set; }

        //Необязательное поле внешней команды, заменить после правок
        [ForeignKey("ExternalSubTeam")]
        public int ExternalSubTeamId { get; set; }
        [Column("ReservedFor"), ForeignKey("MainTeam")]
        public int? MainTeamId { get; set; }

        public HidingTeam MainTeam { get; set; }
        public List<HidingTeam> HidedTeams { get; set; }
        public ExternalSubTeam ExternalSubTeam { get; set; }
    }
}
