using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_ComGSh")]
    public class ExternalTeamRegime
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_ComGSh")]
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
