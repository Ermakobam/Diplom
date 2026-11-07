using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Health")]
    public class HealthCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID_Health")]
        public byte Id { get; set; }
        public string Name { get; set; }
        public string HealthCategoryGroupName { get; set; }
    }
}
