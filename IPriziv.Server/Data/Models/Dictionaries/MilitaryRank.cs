using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_MilRank")]
    public class MilitaryRank
    {
        [Key, Column("ID_MilRank")]
        public byte Id { get; set; }
        public string Name { get; set; }
        [Column("Name_GC")]
        public string NameGC { get; set; }
        [Column("Short")]
        public string NameShort { get; set; }
        [Column("Short_GC")]
        public string NameShortGC { get; set; }
    }
}
