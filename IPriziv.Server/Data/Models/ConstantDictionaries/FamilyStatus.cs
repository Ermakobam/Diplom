using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_Family")]
    public class FamilyStatus
    {
        [Key, Column("ID_Family")]
        public byte Id { get; set; }
        public string Name { get; set; }
    }
}
