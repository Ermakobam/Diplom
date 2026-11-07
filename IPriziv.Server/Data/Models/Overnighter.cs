using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Overnighters")]
    public class Overnighter
    {
        [Key]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }

        [ForeignKey("MilitaryOffice")]
        public byte? MilitaryOfficeId { get; set; }
        [ForeignKey("Overnight")]
        public int OvernightId { get; set; }
        [ForeignKey("Recruit")]
        public int? RecruitId { get; set; }

        public MilitaryOffice MilitaryOffice { get; set; }
        public Overnight Overnight { get; set; }
        public Recruit Recruit { get; set; }
    }
}
