using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ControlCallTelegram")]
    public class ControlTelegram
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(15)]
        public string Code { get; set; }
        public DateTime InformBefore { get; set; }
        public DateTime OverridedCreatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime CalledAt { get; set; }


        [ForeignKey("MilitaryOffice")]
        public byte MilitaryOfficeId { get; set; }


        public MilitaryOffice MilitaryOffice { get; set; }
        public List<ControlTelegramPerson> TelegramPersons { get; set; }
        public List<ControlPerson> Persons { get; set; }
    }
}
