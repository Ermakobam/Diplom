using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("S_ComManager")]
    public class GatheredTeamManager
    {
        [Key, Column("ID_ComManager")]
        public short Id { get; set; }
        [Column("LastN")]
        public string LastName { get; set; }
        [Column("FirstN")]
        public string FirstName { get; set; }
        [Column("MiddleN")]
        public string MiddleName { get; set; }
        [Column("fullname")]
        public string ShortFullName { get; set; }
        [Column("fordoc")]
        public string ShortFullNameGC { get; set; }
        [Column("full_gc")]
        public string FullAliasGC { get; set; }
        [Column("Notes")]
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        [Column("seria")]
        public string PersonDocumentSeries { get; set; }
        [Column("nomer")]
        public string PersonDocumentNumber { get; set; }
        public string PersonPasportSeries { get; set; }
        public string PersonPasportNumber { get; set; }
        [Column("gotin")]
        public string PersonDocumentIssuedAt { get; set; }
        [Column("docum")]
        public string PersonDocumentIssuer { get; set; }
        [Column("doctype")]
        public string PersonDocumentType { get; set; }
        [Column("ofits")]
        public int OfficerCount { get; set; }
        [Column("prap")]
        public int EnsignCount { get; set; }
        [Column("serj")]
        public int SergeantCount { get; set; }
        [Column("comdate")]
        public string BornCity { get; set; }
        [Column("BirthDate")]
        public DateTime? BirthDate { get; set; }
        [Column("vchaddr")]
        public string MilitaryUnitAddress { get; set; }
        [Column("indate")]
        public DateTime? InDate { get; set; }
        [Column("outdate")]
        public DateTime? OutDate { get; set; }
        [Column("vchindef")]
        public string MilitaryUnitCode { get; set; }
        [Column("comspgm")]
        public int? InternalTeamId { get; set; }


        [Column("ID_MilRank"), ForeignKey("MilitaryRank")]
        public byte? MilitaryRankId { get; set; }

        public MilitaryRank MilitaryRank { get; set; }
        public List<TeamDispatching> TeamDispatchings { get; set; }
        public List<TeamManagerEscort> ManagerEscorts { get; set; }
    }
}
