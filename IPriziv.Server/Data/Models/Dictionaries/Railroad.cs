using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Railroad")]
    public class Railroad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID_Railroad")]
        public byte Id { get; set; }
        public string Name { get; set; }
        [Column("Name_GC")]
        public string NameGC { get; set; }
    }
}
