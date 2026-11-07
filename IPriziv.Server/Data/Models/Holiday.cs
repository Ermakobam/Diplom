using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Holiday")]
    public class Holiday
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
