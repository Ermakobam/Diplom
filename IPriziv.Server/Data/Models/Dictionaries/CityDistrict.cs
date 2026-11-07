using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_District")]
    public class CityDistrict
    {
        [Key, Column("ID_District")]
        public byte Id { get; set; }
        public string Name { get; set; }
        [Column("Short")]
        public string ShortName { get; set; }
    }
}
