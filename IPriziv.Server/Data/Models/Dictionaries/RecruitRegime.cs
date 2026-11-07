using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Fate")]
    public class RecruitRegime
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_Fate")]
        public short Id { get; set; }
        public string Name { get; set; }
    }
}
