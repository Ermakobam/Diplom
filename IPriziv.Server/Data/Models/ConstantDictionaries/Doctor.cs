using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_Doctor")]
    public class Doctor
    {
        [Key, Column("ID_Doctor")]
        public short Id { get; set; }
        public string Name { get; set; }
    }
}
