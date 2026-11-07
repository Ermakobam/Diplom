using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ControlTelegramPersons")]
    public class ControlTelegramPerson
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string FirstName { get; set; }
        [Required, MaxLength(150)]
        public string LastName { get; set; }
        [MaxLength(150)]
        public string MiddleName { get; set; }

        public DateTime BirthDate { get; set; }
        public bool IsReportRecieved { get; set; }
        [ForeignKey("ControlPerson")]
        public int? ControlPersonId { get; set; }
        [ForeignKey("ControlTelegram")]
        public int ControlTelegramId { get; set; }


        public ControlPerson ControlPerson { get; set; }
        public ControlTelegram ControlTelegram { get; set; }
    }
}
