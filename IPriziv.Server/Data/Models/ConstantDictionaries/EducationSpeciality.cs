using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_EducationSpec")]
    public class EducationSpeciality
    {
        [Key, Column("p00")]
        public string Id { get; set; }
        [Column("p01")]
        public string Name { get; set; }
    }
}
