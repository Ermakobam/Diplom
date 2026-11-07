using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Education")]
    public class EducationLevel
    {
        [Key, Column("ID_Education")]
        public byte Id { get; set; }
        public string Name { get; set; }
        public string EducationLevelGroupName { get; set; }

        public string EducationOverride { get; set; }
    }
}
