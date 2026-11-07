using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries;

[Table("S_MilSub")]
public class MilitarySubject
{
    [Key]
    [Column("ID_MilSub")]
    public int Id { get; set; }

    public string Name { get; set; }
}
