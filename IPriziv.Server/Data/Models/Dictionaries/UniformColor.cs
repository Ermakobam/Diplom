using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_UniformColor")]
    public class UniformColor
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string CssColor { get; set; }
    }
}
