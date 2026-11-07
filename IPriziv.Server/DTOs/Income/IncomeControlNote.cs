using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlNote
    {
        [Required(AllowEmptyStrings = false), MaxLength(150)]
        public string LastName { get; set; }
        [MaxLength(150)]
        public string FirstName { get; set; }
        [MaxLength(150)]
        public string MiddleName { get; set; }
        public byte? MilitaryOfficeId { get; set; }
        public DateTime? BirthDate { get; set; }

        [MaxLength(150)]
        public string Demander { get; set; }
        [MaxLength(150)]
        public string Orderer { get; set; }

        [MaxLength(450)]
        public string Note { get; set; }

        public short? MilitaryUnitId { get; set; }
        public byte? MilitaryTypeId { get; set; }
        [MaxLength(150)]
        public string Place { get; set; }

        public int? ControlPersonId { get; set; }
    }
}
