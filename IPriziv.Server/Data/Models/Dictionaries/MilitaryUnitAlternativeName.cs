using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_MilUnitAlternativeNames")]
    public class MilitaryUnitAlternativeName
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("MilitaryUnit")]
        public short MilitaryUnitId { get; set; }

        public MilitaryUnit MilitaryUnit { get; set; }
    }
}
