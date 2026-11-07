using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ControlPerson")]
    public class ControlPerson
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string LastName { get; set; }
        [MaxLength(150)]
        public string FirstName { get; set; }
        [MaxLength(150)]
        public string MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Barcode { get; set; }

        [MaxLength(100)]
        public string State { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsExpected { get; set; }

        public bool IsPersonalFileDeclared { get; set; }
        public bool IsCalled { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? ClarifyBeforeDate { get; set; }
        public byte? RemindPeriod { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        [MaxLength(450)]
        public string Note { get; set; }


        [ForeignKey("MilitaryOffice")]
        public byte? MilitaryOfficeId { get; set; }
        [ForeignKey("Recruit")]
        public int? RecruitId { get; set; }

        public Recruit Recruit { get; set; }
        public MilitaryOffice MilitaryOffice { get; set; }
        public List<ControlRecruit> ControlRecruits { get; set; }
        public List<ControlPersonLog> ControlStatusLogs { get; set; }
        public List<ControlTelegram> Telegrams { get; set; }
    }
}
