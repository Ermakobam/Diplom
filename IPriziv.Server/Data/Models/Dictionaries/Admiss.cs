using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Admiss")]
    public class Admiss
    {
        [Key]
        [Column("ID_Admiss")]
        public byte Id { get; set; }
        public string Name { get; set; }
    }
}
