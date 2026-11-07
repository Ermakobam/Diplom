using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_ROSTO")]
    public class MilitaryDebtSpeciality
    {
        [Key]
        [Column("ID_ROSTO")]
        public byte Id { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
}
