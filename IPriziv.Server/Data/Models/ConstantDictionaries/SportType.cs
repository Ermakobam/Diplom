using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_SportType")]
    public class SportType
    {
        [Column("p00")]
        public byte Id { get; set; }
        [Column("p01")]
        public string Name { get; set; }
    }
}
