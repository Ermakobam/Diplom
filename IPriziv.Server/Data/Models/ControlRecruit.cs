using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_PControl")]
    public class ControlRecruit
    {
        [Key, Column("ID_PControl")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }


        [Column("ID_Control"), ForeignKey("ControlList")]
        public short ControlListId { get; set; }
        [ForeignKey("ControlPerson")]
        public int ControlPersonId { get; set; }
        [Column("ID_OVK"), ForeignKey("MilitaryOffice")]
        public byte? MilitaryOfficeId { get; set; }


        public ControlList ControlList { get; set; }
        public ControlPerson ControlPerson { get; set; }
        public MilitaryOffice MilitaryOffice { get; set; }
    }
}
