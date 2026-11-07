using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_CivilSpeciality")]
    public class CivilSpeciality
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
