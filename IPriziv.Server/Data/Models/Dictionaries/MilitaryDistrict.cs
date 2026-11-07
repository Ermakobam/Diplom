using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_MilDist")]
    public class MilitaryDistrict
    {
        [Key]
        [Column("ID_MilDist")]
        public byte Id { get; set; }
        public string Name { get; set; }
        [Column("Name_GC")]
        public string NameGC { get; set; }
        public string ShortName { get; set; }
        public int OrderIndex { get; set; }
    }
}
