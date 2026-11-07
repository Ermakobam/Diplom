using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models
{
    [Table("T_BIO")]
    public class Biometric
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("date")]
        public DateTime PassedAt { get; set; }

        [Column("idPeople"), ForeignKey("Recruit")]
        public int RecruitId { get; set; }

        public Recruit Recruit { get; set; }
    }
}
