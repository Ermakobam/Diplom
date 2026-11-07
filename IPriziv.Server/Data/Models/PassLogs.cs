using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//ПРОПУСКИ - НЕ ДОДЕЛАНО, см. pass-tables

//namespace IPriziv.Server.Data.Models {

//    [Table("T_Pass_Log")]
//    public class PassLogs {

//        [Key, Column("plog_id")]
//        public int passLogId { get; set; }

//        [Column("plog_date")]
//        public DateTime createdAt { get; set; }

//        [ForeignKey("Pass"), Column("pass_id")]
//        public int? passId { get; set; }
//        [ForeignKey("GatheredTeamManager"), Column("comManager_id")]
//        public short? comManagerId { get; set; }
//        [ForeignKey("TeamManagerEscort"), Column("teamManagerEscort_id")]
//        public int? teamManagerEscortId { get; set; }

//        public PassState PassState { get; set; }
//        public GatheredTeamManager GatheredTeamManager { get; set; }
//        public TeamManagerEscort TeamManagerEscort { get; set; }

//    }
//}
